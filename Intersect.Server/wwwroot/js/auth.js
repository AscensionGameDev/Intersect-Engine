/**
 * @param {string} password
 * @returns {Promise<string>}
 */
async function intersectEncodePassword(password) {
	const encodedInput = new TextEncoder().encode(password);
	const digestBuffer = await window.crypto.subtle.digest('sha-256', encodedInput);
	const digestArray = new Uint8Array(digestBuffer);
	const digest = [...digestArray].map(
		/** @param {number} byte */
		(byte) => `00${byte.toString(16)}`.slice(-2)
	).join('');
	return digest;
}

/**
 * @param {HTMLFormElement} loginFormElement
 * @param {number} minimumPasswordLength
 */
function prepareLoginForm(loginFormElement, minimumPasswordLength = 4) {
	if (loginFormElement?.tagName !== 'FORM') {
		throw new Error('Not a form element');
	}

	loginFormElement.addEventListener('htmx:confirm', async (evt) => {
		evt.preventDefault();

		/** @type {HTMLInputElement} */
		const inputEncodedPassword = loginFormElement.querySelector('input[type=hidden][name=encoded-password]');
		if (inputEncodedPassword?.tagName !== 'INPUT') {
			throw new Error('Hidden encoded password input is missing');
		}

		/** @type {HTMLInputElement} */
		const inputPassword = loginFormElement.querySelector('input[type=password][name=password]');
		if (inputPassword?.tagName !== 'INPUT') {
			throw new Error('Password input is missing');
		}

		const password = inputPassword.value?.trim();
		if ((password?.length ?? 0) < minimumPasswordLength) {
			throw new Error('Password too short');
		}

		inputEncodedPassword.value = await intersectEncodePassword(password);
		return evt.detail.issueRequest();
	});

	loginFormElement.addEventListener('htmx:configRequest', (evt) => {
		const { parameters } = evt?.detail ?? {};
		if (typeof parameters !== 'object') {
			evt.preventDefault();
			throw new Error('Parameters not found');
		}

		parameters.password = parameters['encoded-password'];
		delete parameters['encoded-password'];

		parameters['RedirectUri'] = window.location?.searchParams?.get('redirect');
	});
}

/**
 @param {HTMLFormElement} registrationFormElement
 @param {number} minimumPasswordLength
 */
function prepareRegistrationForm(registrationFormElement, minimumPasswordLength = 4) {
	if (registrationFormElement?.tagName !== 'FORM') {
		throw new Error('Not a form element');
	}

	registrationFormElement.addEventListener('htmx:confirm', async (evt) => {
		evt.preventDefault();

		/** @type {HTMLInputElement} */
		const inputEncodedPassword = registrationFormElement.querySelector('input[type=hidden][name=encoded-password]');
		if (inputEncodedPassword?.tagName !== 'INPUT') {
			throw new Error('Hidden encoded password input is missing');
		}

		/** @type {HTMLInputElement} */
		const inputPassword = registrationFormElement.querySelector('input[type=password][name=password]');
		if (inputPassword?.tagName !== 'INPUT') {
			throw new Error('Password input is missing');
		}

		/** @type {HTMLInputElement} */
		const inputConfirmPassword = registrationFormElement.querySelector('input[type=password][name=confirm-password]');
		if (inputConfirmPassword?.tagName !== 'INPUT') {
			throw new Error('Confirm Password input is missing');
		}

		const password = inputPassword.value?.trim();
		if ((password?.length ?? 0) < minimumPasswordLength) {
			throw new Error('Password too short');
		}

		if (password.localeCompare(inputConfirmPassword.value) !== 0) {
			throw new Error('Passwords do not match');
		}

		inputEncodedPassword.value = await intersectEncodePassword(password);
		return evt.detail.issueRequest();
	});

	registrationFormElement.addEventListener('htmx:configRequest', (evt) => {
		const { parameters } = evt?.detail ?? {};
		if (typeof parameters !== 'object') {
			evt.preventDefault();
			throw new Error('Parameters not found');
		}

		parameters.password = parameters['encoded-password'];
		delete parameters['encoded-password'];
		delete parameters['confirm-password'];

		parameters['RedirectUri'] = window.location?.searchParams?.get('redirect');
	});
}
