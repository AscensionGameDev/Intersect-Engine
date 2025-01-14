function kebabToCamelCase(kebab) {
	const characters = [...kebab];

	for (let index = 0; index < characters.length; ++index) {
		const character = characters[index];
		if (character !== '-') {
			continue;
		}

		characters.splice(index, 2, characters[index + 1]?.toLocaleUpperCase());
	}

	return characters.join('');
}

function sanitizeNotEmpty(str) {
	if (typeof str !== 'string' || (str = str.trim()).length < 1) {
		return null;
	}

	return str;
}

/**
 *
 * @param {string} str
 * @param {string} chr
 * @returns {number}
 */
function countCharacterOccurrences(str, chr) {
	if (typeof chr !== 'string' || chr.length !== 1) {
		throw new Error(`Expected a single character but received '${chr}'`);
	}

	if (typeof str !== 'string' || str.length < 1) {
		return 0;
	}

	let occurrences = 0;
	for (let index = 0; index < str.length; ++index) {
		if (str[index] === chr) {
			++occurrences;
		}
	}
	return occurrences;
}

class DeferRenderElement extends HTMLElement {
	static #observer = new IntersectionObserver((entries, observer) => {
		for (const entry of entries) {
			if (!entry.isIntersecting) {
				continue;
			}

			if (!(entry.target instanceof DeferRenderElement)) {
				console.error(`DeferRenderElement's intersection observer is for some reason observing a ${entry.target.tagName}`);
				continue;
			}

			/** @type {DeferRenderElement} */
			const deferRenderElement = entry.target;
			deferRenderElement.#onVisible();
		}
	});

	static observedAttributes = ['tag'];

	/** @type {string} */
	#tag;

	get tag() {
		return this.#tag;
	}

	set tag(value) {
		if (this.#tag === value) {
			return;
		}

		this.#tag = value;
	}

	connectedCallback() {
		console.debug(`Connecting to intersection observer`, this);
		DeferRenderElement.#observer.observe(this);
	}

	disconnectedCallback() {
		console.debug(`Disconnecting from intersection observer`, this);
		DeferRenderElement.#observer.unobserve(this);
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (oldValue === newValue) {
			return;
		}

		const camelName = kebabToCamelCase(name);
		switch (name) {
			case 'tag':
				this[camelName] = newValue;
				break;

			default:
				this[name] = newValue;
				break;
		}
	}

	#onVisible() {
		const attributesToPropagate  = this.getAttributeNames().filter(name => name !== 'tag');
		const attributesString = attributesToPropagate.map(name => `${name}="${this.getAttribute(name)}"`).join(' ');
		this.outerHTML = `<${this.tag} ${attributesString}>${this.innerHTML}</${this.tag}>`;
	}
}

customElements.define('defer-render', DeferRenderElement);

