# Licensing

## Making a game?

In short, you are allowed to create games with Intersect and distribute, sell, or otherwise make money off your games without paying any licensing fees to the development community of Intersect, and without disclosing the source code of your game to your users.

## Making free and public improvements to the engine?

We ask that if you are making improvements to the engine that you do not intend to charge for, that you take a look at our [guidelines for contributing](/CONTRIBUTING.md) to the engine, and if your improvements meet the spirit of the engine please work with us on integrating your changes into the engine itself.

## Commercializing improvements to the engine?

It is recommended that you read this document (and linked license files) in full before making any sales of your improvements to ensure you are not violating any licenses.

While we support other developers using their skills to make a living by making improvements to Intersect, as this is an open source project we do have several of the components of Intersect licensed in a way that requires that you disclose any source modifications to your customers (but not the general public), as they are not required to be publicly distributed (e.g. the server is not distributed to players).

Please see [below](#non-publicly-distributed-compilables-gpl-v3) for more details.

## Further details

Intersect as a whole is divided into several different projects, each with their own licenses based on their expected distribution.

You can view the license for each individual project by clicking any of the links below.

### Publicly distributed compilables (MIT)

_Licensed under MIT to allow for public distribution of binaries without disclosing any source code modifications._

#### [Intersect (Core)/LICENSE.md](/Intersect%20(Core)/LICENSE.md)
The backbone library that contains (most) of the common code between all of the projects.

#### [Intersect.Building/LICENSE.md (GPL v3)](/Intersect.Building/LICENSE.md)
The build scripts library that is used in the compilation of other projects.

#### [Intersect.Network/LICENSE.md (MIT)](/Intersect.Network/LICENSE.md)
The common networking library for all "client" applications (both the client and the editor).

#### [Intersect.Client/LICENSE.md (MIT)](/Intersect.Client/LICENSE.md)
The game client application that is intended to be distributed to players.

#### [Intersect.Client.Framework/LICENSE.md (MIT)](/Intersect.Client.Framework/LICENSE.md)
The game client framework that is intended to be distributed to players.

### Non-publicly distributed compilables (GPL v3)

_Licensed under GPL v3 to dissuade the sale of improvements without disclosing source code modifications to purchasers._

#### [Intersect.Editor/LICENSE.md (GPL v3)](/Intersect.Editor/LICENSE.md)
The game editor application that content developers for games will use to do their jobs, but not something a player needs access to.

#### [Intersect.Server/LICENSE.md (GPL v3)](/Intersect.Server/LICENSE.md)
The server application that is not publicly distributed and is running in secured environments.

#### [Intersect.Server.Framework/LICENSE.md (MIT)](/Intersect.Server.Framework/LICENSE.md)
The game server framework that is not publicly distributed and is used in secured environments.





