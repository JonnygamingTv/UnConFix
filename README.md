# UnConFix
## Unturned Console Fix
Unturned 3 Dedicated Server Console Fix


Unturned Console STDIN does not react on Linux, specifically tested on Xubuntu, therefor I made this (mid-2021).
Mainly useful when using child_process in e.g nodejs: https://nodejs.org/api/child_process.html#child_processspawncommand-args-options
```js
let cp = require('child_process');

let childprocess = cp.spawn("./ServerHelper.sh +secureserver/server", {cwd:"/home/unturned/"});

setInterval(function(){
  childprocess.stdin.write("command\n")
}, 60000);
```
(example Node.JS code)


Nelson did help by telling what should be used (CommandWindow, etc) when I asked him by email! :)



UnConFix = Source project

UnConFixModule = Module to be put in your Modules folder
