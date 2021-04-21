# Origin Repository

[HandyOrg Repositroy](https://github.com/HandyOrg/HandyScreenshot)

# Screen Paste Command Tool
 - 基于HandyScreenshot，由于Snipaste后台贮存以及截屏延迟导致无法将其作为命令行工具使用，因此基于该项目做的命令行工具强化  
 - 性能原因，后期可能用c++改写

# Usage

Enter Key: paste screenshot to clipboard

Exit Code:   
``` json
{
    "1": "Success Paste",
    "0": "Exit",
    "Others": "Fail"
}
```

``` js
exec(`paste.exe`,(exitcode)=>{
    if (exitcode){
        // Your code
    }else{
        // Handle errors
    }
})
```

# Lisence 
[MIT](./LICENSE)



