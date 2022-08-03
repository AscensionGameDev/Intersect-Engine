using Newtonsoft.Json;

namespace Intersect.Localization.Common;

public partial class CommonLicensingNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString CopyrightAuthor = @"Ascension Game Dev";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString CopyrightYear = @"2022";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public readonly LocalizedString GplV3LongNotice = @"Copyright (C) {00}  {01}

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public readonly LocalizedString GplV3ShortNotice = @"{02}  Copyright (C) {00}  {01}
This program comes with ABSOLUTELY NO WARRANTY; for details type `show w'.
This is free software, and you are welcome to redistribute it
under certain conditions; type `show c' for details.";
}
