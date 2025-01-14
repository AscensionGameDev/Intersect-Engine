class ManifestViewerElement extends HTMLElement {
	static observedAttributes = ['label', 'type'];

	#attached = false;

	/** @type {number | null} */
	#raf = null;

	/** @type {HTMLElement} */
	#container = null;

	/** @type {HTMLHeadingElement} */
	#labelElement = null;

	/** @type {HTMLElement} */
	#entriesContainer = null;

	/** @type {HTMLTemplateElement} */
	#entryTemplate = null;

	/** @type {HTMLElement[]} */
	#entryElements = [];

	/** @type {string} */
	#label;

	/** @type {string} */
	#type;

	/**
	 * @typedef {{
	 * 	Path: string,
	 * 	Checksum: string,
	 * 	Size: number,
	 * }} ManifestEntry
	 *
	 * @typedef {{
	 * 	Files: ManifestEntry[],
	 * 	TrustCache: boolean,
	 * }} Manifest
	 */

	/** @type {Manifest | null} */
	#manifest = null;

	constructor() {
		super();

		this.attachShadow({ mode: 'open' });
	}

	get label() {
		return this.#label;
	}

	set label(value) {
		if (this.#label === value) {
			return;
		}

		this.#label = value;
		this.#rerender();
	}

	get type() {
		return this.#type;
	}

	set type(value) {
		if (this.#type === value) {
			return;
		}

		this.#type = value;

		fetch(`/assets/${this.#type}/update.json`).then(async (response) => {
			if (this.#type !== value) {
				console.debug('`type` changed before the request completed, dropping response');
				return;
			}

			if (!response.ok) {
				console.error(response.status, response.statusText);
				return;
			}

			try {
				/** @type {Manifest} */
				const manifest = await response.json();
				this.#manifest = manifest;
			} catch (error) {
				console.error(error);
				return;
			}

			this.#rerender();
		}).catch(console.error);
	}

	connectedCallback() {
		this.#attached = true;
		this.#render();
	}

	disconnectedCallback() {
		this.#attached = false;
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (oldValue === newValue) {
			return;
		}

		const camelName = kebabToCamelCase(name);
		switch (name) {
			case 'label':
			case 'type':
				this[camelName] = newValue;
				break;

			default:
				this[name] = newValue;
				break;
		}
	}

	#render() {
		if (this.#container === null) {
			/** @type {HTMLTemplateElement} */
			const template = document.querySelector('template#custom-element-manifest-viewer');
			this.shadowRoot.appendChild(template.content.cloneNode(true));

			this.#container = this.shadowRoot.querySelector('article.manifest');

			this.#entryTemplate = this.shadowRoot.querySelector('template#custom-element-manifest-entry');

			this.#labelElement = this.#container.querySelector('.label');
			this.#entriesContainer = this.#container.querySelector('section.entries');
		}

		this.#labelElement.textContent = this.#label;

		const manifest = this.#manifest;
		const entryElements = this.#entryElements;
		if (manifest === null) {
			for (const entryElement of entryElements) {
				this.#entriesContainer.removeChild(entryElement);
			}
		} else {
			const filePaths = manifest.Files.map(f => f.Path);
			/** @type {Map<string, HTMLElement>} */
			const reusedElements = new Map();
			for (const entryElement of entryElements) {
				const entryPath = entryElement.getAttribute('data-path');
				if (!filePaths.includes(entryPath)) {
					this.#entriesContainer.removeChild(entryElement);
					return;
				}

				reusedElements.set(entryPath, entryElement);
			}

			/** @type {string | null} */
			let previousPath = null;
			for (let index = 0; index < manifest.Files.length; ++index) {
				const entry = manifest.Files[index];
				let element = reusedElements.get(entry.Path);
				if (element === undefined) {
					const previousElement = previousPath === null ? undefined : reusedElements.get(previousPath);
					previousPath = entry.Path;

					const insertBefore = previousElement?.nextElementSibling ?? null;

					element = this.#entryTemplate.content.cloneNode(true).querySelector('article.entry');
					element.setAttribute('data-path', entry.Path);
					reusedElements.set(entry.Path, element);
					this.#entriesContainer.insertBefore(element, insertBefore);
				}

				const elementName = element.querySelector('.entry-name');
				if (elementName !== null) {
					elementName.textContent = entry.Path;
				}

				const elementSize = element.querySelector('.entry-size');
				if (elementSize !== null) {
					elementSize.textContent = entry.Size?.toLocaleString() ?? '';
				}

				const elementChecksum = element.querySelector('.entry-checksum');
				if (elementChecksum !== null) {
					elementChecksum.textContent = entry.Checksum ?? '';
				}
			}

			this.#entryElements = [...reusedElements.values()];
		}
	}

	#rerender() {
		if (this.#raf !== null) {
			return;
		}

		if (this.#container === null) {
			return;
		}

		this.#raf = requestAnimationFrame(() => {
			this.#render();
			this.#raf = null;
		});
	}
}

customElements.define('manifest-viewer', ManifestViewerElement);

class AssetBrowserElement extends HTMLElement {
	static observedAttributes = ['auto-refresh-interval'];

	#attached = false;

	/** @type {number | null} */
	#autoRefreshInterval = null;

	/** @type {number | null} */
	#raf = null;

	/** @type {HTMLTableElement} */
	#table = null;

	/** @type {HTMLTableSectionElement} */
	#tableBody = null;

	/** @type {string[]} */
	#expanded = ['client', 'client/resources'];

	/** @type {HTMLTableRowElement[]} */
	#rows = [];

	/** @type {HTMLTemplateElement} */
	#rowTemplate = null;

	/** @type {HTMLInputElement} */
	#selectAllElement = null;

	/** @type {LocalStorageBrowseNode} */
	#tree = { children: [] };

	/** @type {HTMLDialogElement} */
	#uploadDialog = null;

	/** @type {HTMLElement} */
	#uploadFilesDropZone = null;

	/** @type {HTMLInputElement} */
	#uploadInputFiles = null;

	/** @type {HTMLInputElement} */
	#uploadInputFolder = null;

	#refreshTimeout = 0;

	#visible = false;

	/**
	 * @typedef {{
	 * 	Name: string,
	 * 	Path: string,
	 * }} BaseAssetFileSystemInfo
	 */

	/**
	 * @typedef {BaseAssetFileSystemInfo & { Type: 'Directory' }} AssetDirectoryInfo
	 */

	/**
	 * @typedef {{
	 * 	Checksum?: string,
	 * 	Size?: number,
	 * 	Type: 'File',
	 * } & BaseAssetFileSystemInfo} AssetFileInfo
	 */

	/** @typedef {AssetDirectoryInfo | AssetFileInfo} AssetFileSystemInfo */

	/** @typedef {AssetFileSystemInfo['Type']} AssetFileSystemInfoType */

	/**
	 * @typedef {{
	 * 	id: string,
	 * 	parentId: string,
	 * } & AssetFileSystemInfo} BrowseNode
	 */

	/**
	 * @typedef {{
	 *   children?: LocalStorageBrowseNode[],
	 * } & BrowseNode} LocalStorageBrowseNode
	 */

	constructor() {
		super();

		this.attachShadow({ mode: 'open' });
	}

	get autoRefreshInterval() {
		return this.#autoRefreshInterval;
	}

	set autoRefreshInterval(value) {
		if (value === this.#autoRefreshInterval) {
			return;
		}

		this.#autoRefreshInterval = value;
	}

	connectedCallback() {
		this.#attached = true;
		this.#expanded = loadArray('asset-browser_expanded', this.#expanded).filter(
			(v, _, a) => !v.includes('/') || a.includes(v.slice(0, Math.max(0, v.lastIndexOf('/'))))
		);
		this.#tree.children = loadArray('asset-browser_browseTree', this.#tree.children);

		this.#loadNodes(['', ...this.#expanded]).then(this.#render.bind(this));
	}

	#savePreferences() {
		if (!this.#attached) {
			console.warn('Tried to save preferences while not attached')
			return;
		}

		saveObject('asset-browser_expanded', this.#expanded);
	}

	#saveTree() {
		if (!this.#attached) {
			console.warn('Tried to save tree while not attached')
			return;
		}

		saveObject('asset-browser_browseTree', this.#tree.children);
	}

	disconnectedCallback() {
		this.#attached = false;
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (oldValue === newValue) {
			return;
		}

		const camelName = kebabToCamelCase(name);
		switch (name) {
			case 'autoRefreshInterval':
				this[camelName] = newValue;
				break;

			default:
				this[name] = newValue;
				break;
		}
	}

	/**
	 * @param {LocalStorageBrowseNode} node
	 * @param {boolean} omitSelf
	 * @returns {LocalStorageBrowseNode[]}
	 */
	#flatten(node, omitSelf = false) {
		const nodes = Array.isArray(node.children) ? node.children.flatMap(child => this.#flatten(child)) : [];
		if (!omitSelf) {
			nodes.splice(0, 0, node);
		}
		return nodes;
	}

	/**
	 * @param {LocalStorageBrowseNode} node
	 * @param {(LocalStorageBrowseNode) => boolean} predicate
	 * @param {boolean} omitSelf
	 * @returns {LocalStorageBrowseNode[]}
	 */
	#filterTree(node, predicate, omitSelf = false) {
		const nodes = Array.isArray(node.children) ? node.children.filter(predicate).flatMap(child => this.#flatten(child)) : [];
		if (!omitSelf) {
			nodes.splice(0, 0, node);
		}
		return nodes;
	}

	/**
	 * @param {LocalStorageBrowseNode} a
	 * @param {LocalStorageBrowseNode} b
	 * @returns {boolean}
	 */
	#compareNodes(a, b) {
		if (a.parentId !== b.parentId) {
			const aSegments = a.Path.split('/');
			const bSegments = b.Path.split('/');

			if (aSegments.length === bSegments.length) {
				return a.id.localeCompare(b.id);
			}

			if (aSegments.length < bSegments.length) {
				return a.id.localeCompare(b.parentId);
			}

			return a.parentId.localeCompare(b.id);
		}
		const comparison = a.parentId.localeCompare(b.parentId);
		if (comparison !== 0) {
			return comparison;
		}

		if (a.Type === 'File') {
			if (b.Type !== 'File') {
				return -1;
			}
		} else if (b.Type === 'File') {
			return 1;
		}

		return a.id.localeCompare(b.id);
	}

	/**
	 * @param {MouseEvent} event
	 */
	#onDirectoryClicked(event) {
		/** @type {HTMLInputElement} */
		const target = event.target;
		const id = target.getAttribute('data-id');
		if (typeof id !== 'string' || id.trim().length < 1) {
			return;
		}

		const path = target.getAttribute('data-path');
		if (typeof path !== 'string' || path.trim().length < 1) {
			return;
		}

		const existingChildren = [...this.#tableBody.querySelectorAll(`tr[data-parent-id^="${id}"]`)];
		if (target.checked) {
			for (const child of existingChildren) {
				const childParentId = child.getAttribute('data-parent-id');
				const childParentPath = decodeURIComponent(childParentId);
				if (childParentId === id || this.#expanded.includes(childParentPath)) {
					child.removeAttribute('data-parent-collapsed');
				}
			}
			this.#loadNode(path).then(() => this.#rerender());
			if (!this.#expanded.includes(path)) {
				this.#expanded.push(path);
			}
		} else {
			for (const child of existingChildren) {
				child.setAttribute('data-parent-collapsed', '');
			}
			let indexOfPath;
			while (-1 < (indexOfPath = this.#expanded.indexOf(path))) {
				this.#expanded.splice(indexOfPath, 1);
			}
		}
		this.#savePreferences();
	}

	#render() {
		if (this.#table === null) {
			/** @type {HTMLTemplateElement} */
			const template = document.querySelector('template#custom-element-asset-browser');
			this.shadowRoot.appendChild(template.content.cloneNode(true));

			this.#uploadDialog = this.shadowRoot.querySelector('dialog#asset-file-upload');
			this.#table = this.shadowRoot.querySelector('table');
			this.#tableBody = this.#table.querySelector('tbody');

			this.#selectAllElement = this.#table.querySelector('input[type=checkbox][name=select-all]');

			this.#rowTemplate = this.shadowRoot.querySelector('template#browse-row');

			this.#uploadFilesDropZone = this.#uploadDialog.querySelector('section.file-dropper');
			this.#uploadInputFiles = this.#uploadDialog.querySelector('input[name=files]');
			this.#uploadInputFolder = this.#uploadDialog.querySelector('input[name=folder]');

			const form = this.#uploadDialog.querySelector('form');
			htmx.process(form);

			const uploadCancel = this.#uploadDialog.querySelector('button.cancel');
			const uploadSubmit = this.#uploadDialog.querySelector('button.submit');

			form.addEventListener('htmx:afterRequest', (evt) => {
				this.#loadNode(evt.detail.requestConfig.parameters.folder).then(this.#rerender.bind(this));
				this.#uploadDialog.close();
			});

			uploadCancel.addEventListener('click', evt => {
				evt.preventDefault();
				this.#uploadDialog.close();
			});

			const updateSubmitDisablement = () => {
				const files = this.#uploadInputFiles.files.length;
				if (files < 1) {
					uploadSubmit.disabled = true;
					return;
				}

				const folder = this.#uploadInputFolder.value?.trim() ?? '';
				uploadSubmit.disabled = folder.length < 1;
			};

			this.#uploadInputFiles.addEventListener('change', updateSubmitDisablement);
			this.#uploadInputFolder.addEventListener('change', updateSubmitDisablement);

			this.#uploadFilesDropZone.addEventListener('dragenter', evt => evt.preventDefault());
			this.#uploadFilesDropZone.addEventListener('dragover', evt => evt.preventDefault());

			this.#uploadFilesDropZone.addEventListener('drop', evt => {
				evt.preventDefault();

				this.#uploadInputFiles.files = evt.dataTransfer.files;
			});

			updateSubmitDisablement();

			const startAutoRefresh = () => {
				const autoRefreshInterval = this.#autoRefreshInterval;
				if (this.#refreshTimeout !== 0 || !this.#visible || autoRefreshInterval === null) {
					return;
				}

				this.#refreshTimeout = setTimeout(async () => {
					if (!this.#visible) {
						clearTimeout(this.#refreshTimeout);
						return;
					}

					const nodes = this.#filterTree(
						this.#tree,
						node => node.Type === 'Directory'
					);
					const pathsToUpdate = nodes.map(({ Path }) => Path);
					await this.#loadNodes(pathsToUpdate);
					startAutoRefresh();
				}, autoRefreshInterval);
			}

			document.addEventListener('visibilitychange', () => {
				this.#visible = document.visibilityState === 'visible';

				if (this.#visible) {
					startAutoRefresh();
				} else {
					clearTimeout(this.#refreshTimeout);
				}
			});

			startAutoRefresh();
		}

		const nodes = this.#flatten(this.#tree, true);
		nodes.sort(this.#compareNodes);
		console.debug('sorted nodes', nodes.map(n => n.id));

		const nodeMetadataMap = new Map(nodes.map(node => {
			const depth = countCharacterOccurrences(node.Path, '/');
			const nodeIndex = nodes.indexOf(node);
			const nextNode = nodes.findLast((n, i) => nodeIndex < i && n.parentId === node.parentId) ?? null;
			const lastChild = nextNode === null;
			return [
				node.id,
				{
					depth,
					lastChild,
				},
			];
		}));

		/**
		 *
		 * @param {HTMLTableRowElement} row
		 * @param {LocalStorageBrowseNode} node
		 */
		function updateRow(row, node) {
			const nameElement = row.querySelector('.name');
			if (nameElement !== null) {
				nameElement.textContent = node.Name;
			}

			const typeElement = row.querySelector('.type');
			if (typeElement !== null) {
				typeElement.textContent = node.Type;
			}

			const sizeElement = row.querySelector('.size');
			if (sizeElement !== null) {
				sizeElement.textContent = node.Type === 'File' ? node.Size.toLocaleString() : '-';
			}

			const checksumElement = row.querySelector('.checksum');
			if (checksumElement !== null) {
				checksumElement.textContent = node.Type === 'File' ? node.Checksum : '-';
			}

			const nodeMetadata = nodeMetadataMap.get(node.id);
			const { depth, lastChild } = nodeMetadata ?? { depth: -1, lastChild: false };
			const depthNode = row.querySelector('.depth');
			if (depthNode !== null) {
				let depthString = [...new Array(Math.max(0, depth - 1)).fill('\t\t\t').join('')];
				if (depth > 0) {
					// this hack shouldn't be needed but without it all of the non-top level files would be offset by one character
					depthString = [...'\t\t', ...depthString];

					const parentNode = nodes.find(n => n.id === node.parentId) ?? null;

					let depthParentNode = parentNode;
					for (let d = 0; d < depth && depthParentNode !== null; ++d) {
						const depthParentMetadata = nodeMetadataMap.get(depthParentNode.id);
						const depthParentIsLastChild = depthParentMetadata?.lastChild ?? false;
						depthString[depthString.length - (2 + d * 3)] = depthParentIsLastChild ? '\t' : '│';
						const depthParentId = depthParentNode.parentId;
						depthParentNode = nodes.find(n => n.id === depthParentId) ?? null;
					}
				}

				depthString = depthString.map(c => c === '\t' ? '&nbsp;' : c);
				depthNode.innerHTML = depthString.join('');
			}

			row.setAttribute('data-depth', depth);
			row.setAttribute('aria-level', depth);
			if (lastChild) {
				row.setAttribute('data-last-child', 'true');
			} else {
				row.removeAttribute('data-last-child');
			}
		}

		const nodeMap = new Map(nodes.map(n => [n.id, n]));
		console.debug('Known nodes:', [...nodeMap.keys()]);
		for (let existingIndex = 0; existingIndex < this.#rows.length; ++existingIndex) {
			const existingElement = this.#rows[existingIndex];
			const existingElementId = existingElement.getAttribute('data-id');
			if (!existingElementId) {
				console.debug('Missing data-id', existingElement);
				continue;
			}

			/** @type {LocalStorageBrowseNode | null} */
			const node = nodeMap.get(existingElementId) ?? null;
			if (node === null) {
				console.debug(`Missing node for '${existingElementId}'`);
				existingElement.parentElement.removeChild(existingElement);
				continue;
			}

			nodeMap.delete(existingElementId);
			updateRow(existingElement, node);
		}

		const nodesToBeCreated = nodes.filter(n => nodeMap.has(n.id));

		for (const node of nodesToBeCreated) {
			const { id, parentId, Path, Type } = node;
			const nodeIndex = nodes.indexOf(node);
			const previousNodeIndex = nodeIndex - 1;
			const previousNode = nodes[previousNodeIndex] ?? null;
			const previousElement = previousNode === null ? null : (this.#rows.find(r => r.getAttribute('data-id') === previousNode.id) ?? null);
			const nextElement = previousElement?.nextElementSibling;

			const row = this.#rowTemplate.content.cloneNode(true).querySelector('tr');
			row.setAttribute('data-id', id);
			row.setAttribute('data-parent-id', parentId);
			row.setAttribute('data-type', Type);
			updateRow(row, node);

			this.#tableBody.insertBefore(row, nextElement);
			this.#rows.push(row);

			const parentPath = decodeURIComponent(parentId);
			if (parentPath === '' || this.#expanded.includes(parentPath)) {
				row.removeAttribute('data-parent-collapsed');
			} else {
				row.setAttribute('data-parent-collapsed', '');
			}

			if (Type === 'Directory') {
				/** @type {HTMLInputElement} */
				const trigger = row.querySelector('input.browse-node-trigger');
				const triggerLabel = row.querySelector('input.browse-node-trigger+label');
				trigger.id = `browse-node-trigger-${id}`;
				trigger.checked = this.#expanded.includes(Path);
				trigger.setAttribute('data-id', id);
				trigger.setAttribute('data-path', Path);
				triggerLabel.htmlFor = trigger.id;
				trigger.addEventListener('change', this.#onDirectoryClicked.bind(this));
			}

			const actionsNode = row.querySelector('.actions');

			if (actionsNode) {
				const buttonRefresh = actionsNode.querySelector('.button.refresh');
				const buttonUpload = actionsNode.querySelector('.button.upload');
				const buttonCreateFolder = actionsNode.querySelector('.button.create-folder');
				const buttonRename = actionsNode.querySelector('.button.rename');
				const buttonDelete = actionsNode.querySelector('.button.delete');

				if (Type === 'Directory') {
					buttonRefresh?.addEventListener('click', () => {
						this.#loadNode(Path).then(() => this.#rerender());
					});
					buttonUpload?.addEventListener('click', () => {
						this.#uploadInputFolder.value = Path;
						this.#uploadDialog.showModal();
					});
					buttonCreateFolder?.addEventListener('click', () => alert('create folder'));
				} else if (Type === 'File') {
					const anchorDownload = actionsNode.querySelector('a.download');
					anchorDownload.href = `/assets/${Path}`;
					anchorDownload.download = '';
				}

				buttonRename?.addEventListener('click', async () => {
					const lastSegmentIndex = Math.max(0, Path.lastIndexOf('/') + 1);
					const parent = Path.slice(0, lastSegmentIndex);
					const name = Path.slice(lastSegmentIndex);
					const newName = prompt('What would you like to rename this to?', name)?.trim();
					if (!newName || name === newName) {
						return;
					}

					const newPath = [parent, newName].join('/').replace(/\/\//g, '/').replace(/^\/+/, '');
					const moveApiUrl = ['/assets', Path].join('/');
					try {
						const newInfoResponse = await fetch(moveApiUrl, { method: 'POST', headers: { 'Move-To': newPath }});
						if (!newInfoResponse.ok) {
							throw new Error(`${newInfoResponse.status} ${newInfoResponse.statusText}`);
						}

						/** @type {AssetFileSystemInfo} */
						const newInfo = await newInfoResponse.json();

						console.debug(`Moved '${Path}'`, newInfo);
						const node = this.#mapToNode(newInfo);
						this.#updateBrowseTree([node], [id]);
						this.#rerender();
						console.debug(`Refreshed '${Path}'`);
					} catch (error) {
						console.error(error);
					}
				});
				buttonDelete?.addEventListener('click', async () => {
					const result = confirm(`Are you sure you want to delete '${Path}'?`);
					if (!result) {
						return;
					}

					console.debug(`Deleting '${Path}'...`);
					try {
						const deleteResponse = await fetch(`/assets/${Path}`, { method: 'DELETE' });
						if (!deleteResponse.ok) {
							throw new Error(`${deleteResponse.status} ${deleteResponse.statusText}`);
						}

						this.#updateBrowseTree([], [id]);
						await this.#loadNodes([parentPath]);
					} catch (err) {
						console.error(err);
					}
				});
			}
		}

		this.#rows = this.#rows.filter(r => this.#tableBody === r.parentElement);
	}

	#rerender() {
		if (this.#raf !== null) {
			console.debug('Re-render already queued');
			return;
		}

		if (this.#table === null) {
			console.debug('Skipping re-render because table is missing');
			return;
		}

		console.debug('Enqueuing re-render...');
		this.#raf = requestAnimationFrame(() => {
			console.debug('Re-rendering...');
			this.#render();
			this.#raf = null;
		});
	}

	/**
	 * @typedef {{
	 *   debug?: boolean,
	 *   force?: boolean,
	 * }} LoadNodeInit
	 */

	/** @type {Set<string>} */
	#pendingLoadRequests = new Set();

	/**
	 *
	 * @param {string[]} paths
	 * @param {LoadNodeInit} [init]
	 * @returns {Promise<void>}
	 */
	async #loadNodes(paths, init) {
		for (const path of paths) {
			await this.#loadNode(path, init);
			this.#rerender();
		}
	}

	/**
	 * @param {AssetFileSystemInfo} entry
	 * @returns {LocalStorageBrowseNode}
	 */
	#mapToNode(entry) {
		const id = encodeURIComponent(entry.Path);
		const parentId = id.slice(0, Math.max(0, id.lastIndexOf('%2F')));
		/** @type {LocalStorageBrowseNode} */
		const node = {
			...entry,
			id,
			parentId,
		};

		if (node.Type === 'Directory') {
			node.children = [];
		}

		return node;
	}

	/**
	 *
	 * @param {string} path
	 * @param {LoadNodeInit} [init]
	 * @returns {Promise<LocalStorageBrowseNode[]>}
	 */
	async #loadNode(path, init) {
		const { debug = false, force = false } = init ?? {};
		if (!force && this.#pendingLoadRequests.has(path)) {
			return [];
		}

		this.#pendingLoadRequests.add(path);
		try {
			const response = await fetch(`/assets/${path}`.replace(/(\\)|(\/{2,})/g, '/'), {
				headers: {
					accept: 'application/json',
				},
			});

			/** @type {AssetFileSystemInfo[]} */
			const entries = await response.json();
			console.debug('received entries', path, entries);

			const nodesToAdd = entries.map(this.#mapToNode.bind(this));
			const existing = this.#navigateToExisting(this.#tree, path);
			const nodesToRemove = existing?.children.filter(c => nodesToAdd.findIndex(n => n.id === c.id) < 0).map(c => c.id);
			console.debug('updating nodes', path, nodesToRemove, nodesToAdd);
			const updatedTree = this.#updateBrowseTree(nodesToAdd, nodesToRemove);
			console.debug('updated tree', updatedTree);
		} catch (err) {
			console.error(err);
			const updatedTree = this.#updateBrowseTree([], [encodeURIComponent(path)]);
			console.debug('updated tree', updatedTree);
			throw err;
		} finally {
			this.#pendingLoadRequests.delete(path);
		}
	}

	/**
	 *
	 * @param {LocalStorageBrowseNode} root
	 * @param {string} target
	 * @param {boolean} [createMissing]
	 *
	 * @returns {LocalStorageBrowseNode | null}
	 */
	#navigateToParent(root, target, createMissing = false) {
		const parts = target.split('/').filter(p => p.length > 0);

		let current = root;
		for (let depth = 0; current.children && depth < parts.length - 1; ++depth) {
			/** @type {LocalStorageBrowseNode | null} */
			let next = current.children.find(c => c.Name === parts[depth]) ?? null;
			if (next === null && createMissing) {
				console.debug(`Did not find '${parts[depth]}' in ${current.id}'s children ${current.children.map(c => c.Name).join(', ')}`);
				const path = parts.filter((_, i) => i <= depth).join('/');
				next = {
					id: encodeURIComponent(path),
					Name: parts[depth],
					Path: path,
					Type: 'Directory',
					children: [],
				};
				current.children.push(next);
			}
			current = next;
		}

		return current;
	}

	/**
	 *
	 * @param {LocalStorageBrowseNode} root
	 * @param {string} target
	 * @returns {LocalStorageBrowseNode | null}
	 */
	#navigateToExisting(root, target) {
		const parts = target.split('/').filter(p => p.length > 0);

		let current = root;
		for (let depth = 0; Array.isArray(current?.children) && depth < parts.length; ++depth) {
			/** @type {LocalStorageBrowseNode | null} */
			let next = current.children.find(c => c.Name === parts[depth]) ?? null;
			current = next;
		}

		return current;
	}

	/**
	 *
	 * @param {LocalStorageBrowseNode[]} addNodes
	 * @param {string[]} [removeNodeIds]
	 */
	#updateBrowseTree(addNodes, removeNodeIds) {
		console.debug('updateBrowseTree', JSON.parse(JSON.stringify(this.#tree)));
		/** @type {LocalStorageBrowseNode[]} */
		if (Array.isArray(removeNodeIds) && removeNodeIds.length > 0) {
			const sortedNodeIds = removeNodeIds.toSorted();
			for (const nodeId of sortedNodeIds) {
				const parent = this.#navigateToParent(this.#tree, decodeURIComponent(nodeId));
				if (parent === null) {
					console.debug(`No parent for ${nodeId}`);
					continue;
				}

				const indexInParent = parent.children.findIndex(c => c.id === nodeId);
				if (indexInParent < 0) {
					console.debug(`Parent ${parent.id} does not contain ${nodeId}`);
					continue;
				}

				console.debug(`Removing from parent: ${nodeId}`);
				parent.children.splice(indexInParent, 1);
			}
		}

		if (Array.isArray(addNodes) && addNodes.length > 0) {
			const sortedNodes = addNodes.toSorted((a, b) => a?.id?.localeCompare(b?.id) ?? 1);
			for (const node of sortedNodes) {
				const parent = this.#navigateToParent(this.#tree, node.Path, true);
				if (parent === null) {
					console.warn('Parent was null?')
					continue;
				}

				const indexInParent = parent.children.findIndex(c => c.id === node.id);
				if (indexInParent < 0) {
					console.debug(`Adding node '${node.Path}' (did not find ${node.id} in ${parent.id}'s children)`);
					parent.children.push(node);
					continue;
				}

				const existingNode = parent.children[indexInParent] ?? node;
				console.debug(`Updating node '${node.Path}' (${existingNode.children?.length})`);
				parent.children[indexInParent] = Object.assign(existingNode, node, { children: existingNode.children ?? node.children });
				parent.children.sort(this.#compareNodes);
			}
		}

		this.#saveTree();
		return this.#tree.children;
	}
}

/**
 * @param {string} key
 * @param {string} [defaultValue]
 * @returns {string|null}
 */
function loadString(key, defaultValue) {
	const value = localStorage.getItem(key);
	if (value === null) {
		return defaultValue ?? null;
	}
	return value;
}

/**
 * @template TValue
 * @param {string} key
 * @param {TValue[]} [defaultValue]
 * @returns {TValue[]|null}
 */
function loadArray(key, defaultValue) {
	const objectValue = loadObject(key);
	if (Array.isArray(objectValue)) {
		return objectValue;
	}

	return defaultValue ?? null;
}

/**
 * @template TValue
 * @param {string} key
 * @param {TValue} [defaultValue]
 * @returns {TValue|null}
 */
function loadObject(key, defaultValue) {
	const rawValue = loadString(key);
	if (rawValue === null) {
		return defaultValue ?? null;
	}

	return JSON.parse(rawValue);
}

function saveString(key, value) {
	localStorage.setItem(key, value);
}

function saveObject(key, value) {
	const jsonValue = JSON.stringify(value);
	saveString(key, jsonValue);
}

customElements.define('asset-browser', AssetBrowserElement);
