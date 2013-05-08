# OccupOS [![Build Status](http://occupos.morrisoncole.co.uk/app/rest/builds/buildType:bt2/statusIcon)](http://occupos.morrisoncole.co.uk/viewType.html?buildTypeId=bt2&guest=1)

Occupancy OS is a package of extensible, scalable software to be used by large organisations or in large buildings where energy-efficiency wants to be increased or occupancy related data wants to be measured.

**Please note:** *Due to the early stage of the project, things are still in the process of changing. We try to keep this file up-to-date, but it's unfortunately not always possible. If you have any issues setting up OccupOS, you can file an [issue](https://github.com/OccupOS/OccupOS/issues) or get in touch (mail@occupos.org).*

## User Guide

Will be updated soon.

## Developer Guide

The OccupOS.sln in this submodule references projects and files in both the OccupOSCloud and OccupOSNode submodules, you'll need to clone them as well using something like:

    git clone git://github.com/OccupOS/OccupOS.git OccupOS
    cd OccupOS
    git submodule init --update

Or for version 1.6.5 of Git and later:

    git clone --recursive git://github.com/OccupOS/OccupOS.git OccupOS

### Setup

Although you are free to use whatever you like, we suggest that new users use our recommended toolchains until they are familiar with our build and test structure.

**Windows**

1. Microsoft Visual Studio 2012 (preferrably Professional / Ultimate edition).
2. [.NET MF 4.3 (RTM)] (http://netmf.codeplex.com/releases/view/81000).
3. [.NET Gadgeteer Core 2.42.700](http://gadgeteer.codeplex.com/releases/view/105366).

**Linux**

Will be updated soon.

**OS X**

Will be updated soon.


Copyright and license
-------

OccupOS
Occupancy Operating System intelligently uses various (low-powered) sensors to increase energy efficiency, environment quality and track precise occupancy in large buildings in an open, extensible, efficient and social way.
Copyright (C) 2013 Choi Jisang, Cole Morrison, Davis Richy, Kaliyev Daniyar, Padourek Markus

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
