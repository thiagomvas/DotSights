# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="1.4.2"></a>
## [1.4.2](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.4.2) (2024-07-25)

### Bug Fixes

* Add missing check for storage optimizations ([0640080](https://www.github.com/thiagomvas/DotSights/commit/06400801c847f5ceee25569ac59846852adfffc6))
* Grouped Process Names now default to lower case invariant ([eec568a](https://www.github.com/thiagomvas/DotSights/commit/eec568a52e2ea1d6c0342693eb81ace9f137d8cd))

<a name="1.4.1"></a>
## [1.4.1](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.4.1) (2024-07-23)

### Bug Fixes

* CLI now displays full 24 hour table if after 2 pm to prevent crashing ([b997bf7](https://www.github.com/thiagomvas/DotSights/commit/b997bf7ab16671bd345afeaae02c3ffbaecf7110))

<a name="1.4.0"></a>
## [1.4.0](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.4.0) (2024-07-23)

### Features

* CLI now shows graphs for usage for 'today', 'week' and 'today' with day offset ([6e3effe](https://www.github.com/thiagomvas/DotSights/commit/6e3effe2260a00fd8d147fb8d27b74479efb54bf))
* Tracker now tracks daily hourly usage for the last 7 days ([dfc11d2](https://www.github.com/thiagomvas/DotSights/commit/dfc11d25b23c8d4c005365717e8206df292be8f1))

### Bug Fixes

* Display graphs no longer crash when giving a large offset ([77f116a](https://www.github.com/thiagomvas/DotSights/commit/77f116a32ee9eef96f58ef741271c70495a9f7ef))

<a name="1.3.0"></a>
## [1.3.0](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.3.0) (2024-07-13)

### Features

* Add 'config backup' command to create a backup ([01b6167](https://www.github.com/thiagomvas/DotSights/commit/01b61671aa525602c0027909e7b6d2441b3356c7))
* Add 'display overall' command ([19133c3](https://www.github.com/thiagomvas/DotSights/commit/19133c3af8aa15b13b7cd073b5f1b94a416eb36a))
* Add automatic data backups ([132fdb8](https://www.github.com/thiagomvas/DotSights/commit/132fdb869525cfffb62741ba14f81c20b61be9e8))
* Add config options for backup configuration ([db040f2](https://www.github.com/thiagomvas/DotSights/commit/db040f2803c1fa641d9e949920484ef60e975ac3))
* Add config restore command to restore backup data ([c5f2480](https://www.github.com/thiagomvas/DotSights/commit/c5f2480e2f04d155ddcf9cfc01a3f2907f21a7b4))

<a name="1.2.0"></a>
## [1.2.0](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.2.0) (2024-07-07)

### Features

* Add pagination and interactable tables ([7d372cc](https://www.github.com/thiagomvas/DotSights/commit/7d372cc6b051edeb3cceaf1e1214d9816d3c2e0a))

### Bug Fixes

* Weekly Data only shows past 7 real days instead of past 7 days of usage ([2c15037](https://www.github.com/thiagomvas/DotSights/commit/2c15037408fc8755d3f2a011b6c588d860c5b5a5))
* **tracker:** Tracker now reads data from file before saving, preventing overrides ([4f2136c](https://www.github.com/thiagomvas/DotSights/commit/4f2136c1003ce580de9cf7dc3c7fabcaed763920))

<a name="1.1.0"></a>
## [1.1.0](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.1.0) (2024-06-29)

### Features

* Squash - Merge multiple entries into one using Regex ([7dcbef5](https://www.github.com/thiagomvas/DotSights/commit/7dcbef5c29cb2e898c2a462b2ecf775dad0f075a))

### Bug Fixes

* Squash with process names wasn't implemented ([d2e2453](https://www.github.com/thiagomvas/DotSights/commit/d2e2453f98f68b9d38f9e42cacc7ec6969a5c2fe))
* Table Headers no longer broken ([5c9c88d](https://www.github.com/thiagomvas/DotSights/commit/5c9c88d5709e40409baa6b5d2c6ea109ee895026))
* Tracker now fetches data and settings before saving to prevent overriding ([addaf83](https://www.github.com/thiagomvas/DotSights/commit/addaf830204d5b8cdd69dd13cbf4cf2f65822794))
* **tracker:** Calculate delay instead of a fixed 1 second delay to give more accurate results ([1f8a2c4](https://www.github.com/thiagomvas/DotSights/commit/1f8a2c43b76d9e417e6a90bd5f4d4cbe4e676dcf))

<a name="1.0.1"></a>
## [1.0.1](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.0.1) (2024-06-28)

### Bug Fixes

* fix CI CD ([28a3b11](https://www.github.com/thiagomvas/DotSights/commit/28a3b11c956263424cf8ae4be16949a583bf1cce))

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/thiagomvas/DotSights/releases/tag/v1.0.0) (2024-06-28)

### Bug Fixes

* Startup option would not work properly ([3b963cc](https://www.github.com/thiagomvas/DotSights/commit/3b963cc0fff457059ec2d8380f1b6dbcd820c751))

