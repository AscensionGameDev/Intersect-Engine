class TabContentVisibleEvent extends CustomEvent {
	static NAME = 'tabcontentvisible';

	constructor() {
		super(TabContentVisibleEvent.NAME, {
			detail: {}
		});
	}
}

class TabContentElement extends HTMLElement {
	static observedAttributes = [
		'tab-icon',
		'tab-id',
		'tab-label'
	];

	/** @type {HTMLElement} */
	#contentContainer;

	/** @type {boolean} */
	#selected = false;

	/** @type {string?} */
	#tabIcon;

	/** @type {string?} */
	#tabId;

	/** @type {string?} */
	#tabLabel;

	/** @type {boolean} */
	get selected() {
		return this.#selected;
	}

	/** @param {boolean} value */
	set selected(value) {
		if (this.#selected === value) {
			return;
		}

		this.#selected = typeof value === 'boolean' ? value : Boolean(value);
		this.#contentContainer?.setAttribute('aria-hidden', !this.#selected);
		if (value) {
			this.classList.add('selected');
			this.dispatchEvent(new TabContentVisibleEvent());
		} else {
			this.classList.remove('selected');
		}
	}

	/** @type {string} */
	get tabIcon() {
		return this.#tabIcon;
	}

	/** @param {string?} value */
	set tabIcon(value) {
		this.#tabIcon = value;
	}

	/** @type {string} */
	get tabId() {
		return this.#tabId;
	}

	/** @param {string?} value */
	set tabId(value) {
		this.#tabId = value;
	}

	/** @type {string} */
	get tabLabel() {
		return this.#tabLabel;
	}

	/** @param {string?} value */
	set tabLabel(value) {
		this.#tabLabel = value;
	}

	constructor() {
		super();

		this.attachShadow({
			mode: 'open'
		});
	}

	#render() {
		/** @type {HTMLTemplateElement} */
		const template = document.querySelector('template#custom-element-tab-content');
		this.shadowRoot.appendChild(template.content.cloneNode(true));

		this.#contentContainer = this.shadowRoot.querySelector('div.content');
		this.#contentContainer.setAttribute('aria-hidden', !this.#selected);
	}

	connectedCallback() {
		this.#render();
	}

	disconnectedCallback() {
	}

	adoptedCallback() {
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (oldValue === newValue) {
			return;
		}

		const camelName = kebabToCamelCase(name);
		switch (name) {
			case 'tab-icon':
			case 'tab-id':
			case 'tab-label':
				this[camelName] = newValue;
				break;

			default:
				this[name] = newValue;
				break;
		}
	}
}

customElements.define('tab-content', TabContentElement);

class TabSetElement extends HTMLElement {
	static observedAttributes = [];

	/** @type {HTMLElement} */
	#elementContainer;

	/** @type {HTMLElement} */
	#triggerContainer;

	/** @type {HTMLTemplateElement} */
	#triggerTemplate;

	/** @type {HTMLSlotElement} */
	#slotContents;

	/** @type {TabContentElement[]} */
	#contentElements = [];

	/** @type {number} */
	#selectedIndex = 0;

	constructor() {
		super();

		this.attachShadow({
			mode: 'open'
		});
	}

	get selectedIndex() {
		return this.#selectedIndex;
	}

	/**
	 *
	 * @param {number} value
	 */
	set selectedIndex(value) {
		if (this.#selectedIndex === value) {
			return;
		}

		const currentTab = this.#contentElements[this.#selectedIndex];
		if (currentTab !== undefined) {
			const { tabId } = currentTab;
			currentTab.selected = false;
			const trigger = this.#triggerContainer.querySelector(`input[data-tab-id="${tabId}"]`);
			if (trigger !== null) {
				trigger.checked = false;
			}
		}

		this.#selectedIndex = value;

		const selectedTab = this.#contentElements[this.#selectedIndex];
		if (selectedTab !== undefined) {
			const { tabId } = selectedTab;
			selectedTab.selected = true;
			const trigger = this.#triggerContainer.querySelector(`input[data-tab-id="${tabId}"]`);
			if (trigger !== null) {
				trigger.checked = true;
			}
		}
	}

	#render() {
		/** @type {HTMLTemplateElement} */
		const template = document.querySelector('template#custom-element-tab-set');
		this.shadowRoot.appendChild(template.content.cloneNode(true));

		this.#elementContainer = this.shadowRoot.querySelector('div.container');
		this.#triggerContainer = this.shadowRoot.querySelector('div.triggers');
		this.#triggerTemplate = this.shadowRoot.querySelector('template#trigger');

		this.#slotContents = this.shadowRoot.querySelector('slot:not([name])');

		const onSlotChanged = () => {
			/** @type {TabContentElement[]} */
			const tabs = this.#slotContents.assignedElements().filter(e => e instanceof TabContentElement);

			/** @type {string[]} */
			const tabIds = [];

			for (const tab of tabs) {
				const tabId = tab.tabId?.trim();
				const tabIcon = tab.tabIcon?.trim();

				if (typeof tabId !== 'string' || tabId.length < 1) {
					console.error('Invalid tab, `tab-id` attribute is not set', tab);
					continue;
				}

				const isSelected = this.#selectedIndex === tabIds.length;

				tabIds.push(tabId);

				/** @type {HTMLInputElement | null} */
				let input = this.#triggerContainer.querySelector(`input[data-tab-id="${tab.tabId}"]`);

				/** @type {HTMLLabelElement | null} */
				let label = this.#triggerContainer.querySelector(`label[data-tab-id="${tab.tabId}"]`);

				if (input === null || label === null) {
					if (input !== null) {
						this.#triggerContainer.removeChild(input);
					}

					if (label !== null) {
						this.#triggerContainer.removeChild(label);
					}

					/** @type {DocumentFragment} */
					const nodes = this.#triggerTemplate.content.cloneNode(true);
					input = nodes.querySelector('input');
					label = nodes.querySelector('label');

					input.setAttribute('data-tab-id', tabId);
					label.setAttribute('data-tab-id', tabId);

					input.id = `trigger-${tabId}`;
					label.id = `label-${tabId}`;
					label.htmlFor = input.id;

					const [lastTabId] = tabIds.slice(-1);
					const [lastTab] = lastTabId === undefined ? [null] : [...this.#triggerContainer.querySelectorAll(`[data-tab-id="${lastTabId}"]`)];
					const nextSibling = lastTab?.nextSibling;
					this.#triggerContainer.insertBefore(input, nextSibling);
					this.#triggerContainer.insertBefore(label, nextSibling);

					input.addEventListener('change', ({ target }) => {
						if (target?.tagName !== 'INPUT') {
							console.error('Invalid target, expected an `<input />` element', target);
							return;
						}

						/** @type {string | null} */
						const selectedTabId = target.getAttribute('data-tab-id')?.trim();
						if (typeof selectedTabId !== 'string' || selectedTabId.length < 1) {
							console.error('Invalid target, the `<input />` has no `data-tab-id` attribute', target);
							return;
						}

						const nextSelectedIndex = this.#contentElements.findIndex(e => e.tabId === selectedTabId);
						console.info(`Selecting tab ${nextSelectedIndex}`);
						this.#contentElements[this.#selectedIndex]?.classList.remove('selected');
						this.#selectedIndex = nextSelectedIndex;
						this.#contentElements[this.#selectedIndex]?.classList.add('selected');
					});

					label.addEventListener('keypress', evt => {
						if (evt.key === ' ' || evt.key === 'Enter') {
							const { target } = evt;
							const tabId = target.getAttribute('data-tab-id');
							const tabIndex = this.#contentElements.findIndex(tab => tab.tabId === tabId);
							console.debug(`Selecting tab ${tabIndex} (${tabId}) with the keyboard`);
							this.selectedIndex = tabIndex;
						}
					});
				}

				if (typeof tabIcon === 'string' && tabIcon.length > 0) {
					const iconUse = label.querySelector('use');
					iconUse.setAttribute('href', tabIcon);
					iconUse.parentElement.classList.remove('hidden');
				}

				input.checked = isSelected;

				label.querySelector('span').textContent = tab.tabLabel?.trim();

				tab.id = `panel-${tabId}`;
				tab.setAttribute('aria-labelledby', label.id);
				tab.selected = isSelected;

				label.setAttribute('aria-controls', tab.id);
			}

			for (const element of this.#triggerContainer.children) {
				switch (element.tagName) {
					case 'INPUT':
					case 'LABEL':
						if (!tabIds.includes(element.getAttribute('data-tab-id'))) {
							this.#triggerContainer.removeChild(element);
						}
						break;
				}
			}

			this.#contentElements = tabs;
		};

		this.#slotContents.addEventListener('slotchange', onSlotChanged);
	}

	connectedCallback() {
		this.#render();
	}

	disconnectedCallback() {
	}

	adoptedCallback() {
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (oldValue !== newValue) {
			if (name === 'selected-index') {
				this.selectedIndex = newValue;
			} else {
				this[name] = newValue;
			}
		}
	}
}

customElements.define('tab-set', TabSetElement);
