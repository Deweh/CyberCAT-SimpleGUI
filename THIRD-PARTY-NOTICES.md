# Third-party notices

CyberCAT-SimpleGUI's own source code is MIT licensed (see [LICENSE](LICENSE)). The released program also contains the
components below. Two of them, WolvenKit and the Kraken decompressor, are licensed under the GNU GPL version 3, so the
program as distributed (the release zip) is covered by the **GNU General Public License v3.0**. The full license text
is in [licenses/WolvenKit.LICENSE.txt](licenses/WolvenKit.LICENSE.txt).

## Source code

The complete corresponding source for every release is in this repository at the release's tag:

- the editor itself,
- the `WolvenKit` submodule, at the commit the tag points to,
- `patches/WolvenKit.patch`, the changes made to WolvenKit, which the build applies (see `Directory.Build.targets`).
  Each modified WolvenKit file is marked in its first line.

The Kraken decompressor (`kraken.dll`) is built from https://github.com/WolvenKit/kraken.

## Components

| Component | Files | License | Copyright | Source |
|---|---|---|---|---|
| WolvenKit (WolvenKit.Common, WolvenKit.Core, WolvenKit.RED4), modified | `WolvenKit.*.dll` | GPL-3.0 | WolvenKit contributors | https://github.com/WolvenKit/WolvenKit |
| Kraken decompressor ("ooz") | `kraken.dll` | GPL-3.0-or-later | Copyright (C) 2016, Powzix | https://github.com/WolvenKit/kraken |
| K4os.Compression.LZ4 | `K4os.Compression.LZ4.dll` | MIT | Copyright (c) 2017 Milosz Krajewski | https://github.com/MiloszKrajewski/K4os.Compression.LZ4 |
| Serilog | `Serilog.dll` | Apache-2.0 | Copyright © Serilog Contributors | https://github.com/serilog/serilog |
| Semver | `Semver.dll` | MIT | Copyright © 2013-2024 Jeff Walker, Max Hauser | https://github.com/WalkerCodeRanger/semver |
| DynamicData | `DynamicData.dll` | MIT | Copyright (c) Roland Pheasant 2011-2025 | https://github.com/reactiveui/DynamicData |
| System.Reactive | `System.Reactive.dll` | MIT | Copyright (c) .NET Foundation and Contributors | https://github.com/dotnet/reactive |
| CommunityToolkit.Mvvm | `CommunityToolkit.Mvvm.dll` | MIT | (c) .NET Foundation and Contributors | https://github.com/CommunityToolkit/dotnet |
| Microsoft.Extensions.* (Caching, Configuration, DependencyInjection, DependencyModel, Logging, Options, Primitives), System.Reflection.MetadataLoadContext | `Microsoft.Extensions.*.dll`, `System.Reflection.MetadataLoadContext.dll` | MIT | © Microsoft Corporation | https://github.com/dotnet/runtime |

CyberCAT-SimpleGUI started as an offshoot of CyberCAT by SirBitesalot and contributors (MIT, see
[licenses/CyberCAT.Core.LICENSE.txt](licenses/CyberCAT.Core.LICENSE.txt)).

The .NET runtime isn't included in the release. It's installed separately (MIT, © .NET Foundation and Contributors).

## License texts

- GPL-3.0 (WolvenKit, Kraken): [licenses/WolvenKit.LICENSE.txt](licenses/WolvenKit.LICENSE.txt)
- Apache-2.0 (Serilog): [licenses/Serilog.LICENSE.txt](licenses/Serilog.LICENSE.txt)
- MIT (K4os.Compression.LZ4): [licenses/K4os.Compression.LZ4.LICENSE.txt](licenses/K4os.Compression.LZ4.LICENSE.txt)
- MIT (all other MIT components above), with each component's copyright line from the table:

```
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
