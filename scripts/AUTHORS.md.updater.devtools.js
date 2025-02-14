(() => {
    const authors = [
        { username: 'jcsnider', link: 'https://github.com/jcsnider', name: 'JC Snider' },
        { username: 'irokaiser', link: 'https://github.com/irokaiser', name: 'Joe Bridges' },
        { username: 'lodicolo', link: 'https://github.com/lodicolo', name: 'Robbie Lodico' },
    ];

    const maintainers = [
        { username: 'cheshire92', link: 'https://github.com/Cheshire92', name: 'Jocelyn Cobb' },
    ];

    const collaborators = [
        { username: 'alexvild', link: 'https://github.com/AlexVild', name: 'Alex Vild' },
        { username: 'arufonsu', link: 'https://github.com/Arufonsu', name: 'Fernando Arzola Lagos' },
        { username: 'weylonsantana', link: 'https://github.com/WeylonSantana', name: 'Weylon Santana' },
    ];

    const usernamesToIgnore = new Set([
        'AlexVild',
        'alloin', /* listed under a different name */
        'Arufonsu',
        'Cheshire92',
        'apps/dependabot', /* bot */
        'Irokaiser',
        'jcsnider',
        'lodicolo',
        'WeylonSantana'
    ].map(username => username.toLowerCase()));

    const contributorsToAdd = [
        {
            link: '#',
            name: 'Dog from the Kash Shop',
            username: 'dog-from-the-kash-shop', /* used for sorting */
        }
    ]

    const mappedNames = new Map([
        ['airobinnet', 'AIRobin (airobinnet, alloin)'],
        ['redbandana', 'Shenmue (RedBandana)']
    ]);
    
    function getUsername(url) {
        return url.replace('https://github.com/', '').toLowerCase();
    }

    function userToLine({ name, link }) {
        return `[${name}](${link})`;
    }

    const contributorLinks = [...document.querySelector('[app-name=repos-contributors-chart]')?.querySelectorAll('h2 a.prc-Link-Link-85e08') ?? []];
    const allContributors = [
        ...contributorsToAdd,
        ...contributorLinks.map(({ href, textContent }) => {
            const username = getUsername(href);
            return {
                link: href,
                name: mappedNames.get(username) ?? textContent,
                username
            };
        }),
    ];

    const filteredContributors = allContributors.filter(({ username }) => !usernamesToIgnore.has(username));
    const sortedContributors = filteredContributors.toSorted(({ username: a }, { username: b }) => a?.localeCompare(b));

    return {
        'AUTHORS.md': `# Authors

${authors.map(userToLine).join('\n\n')}

# Maintainers

${maintainers.map(userToLine).join('\n\n')}

# Collaborators

${collaborators.map(userToLine).join('\n\n')}

# Contributors

${sortedContributors.map(userToLine).join('\n\n')}`,
        creditsObjects: sortedContributors.map(({ name }) => ({
            "Text": name,
            "Font": "sourcesansproblack",
            "Size": 12,
            "Alignment": "center"
        })),
    };
})()