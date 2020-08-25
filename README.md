[![](https://img.shields.io/github/stars/RomanSoloweow/HistoricalReactiveCommand)](https://github.com/RomanSoloweow/HistoricalReactiveCommand) [![](https://img.shields.io/github/languages/code-size/RomanSoloweow/HistoricalReactiveCommand)](https://github.com/RomanSoloweow/HistoricalReactiveCommand) [![]( https://img.shields.io/github/last-commit/RomanSoloweow/HistoricalReactiveCommand/master)](https://github.com/RomanSoloweow/HistoricalReactiveCommand) [![](https://img.shields.io/github/license/RomanSoloweow/HistoricalReactiveCommand)](https://github.com/RomanSoloweow/HistoricalReactiveCommand)
# HistoricalReactiveCommand
HistoricalReactiveCommand for [ReactiveUI](https://github.com/reactiveui/ReactiveUI).

The creation and logic of work is as similar as possible to conventional [reactive commands](https://www.reactiveui.net/docs/handbook/commands/).

# A Compelling Example
## Simple command with History
 ```C#
    //Registry default history. You can set your IScheduler here
    History.RegistryDefaultHistory();
    
    int myNumber = 0;
    var command = ReactiveCommandEx.CreateWithHistory<int>("adding",
     (number) => { myNumber += number; },
     (number) => { myNumber -= number; });

    //myNumber is 25
    command.Execute(25).Subscribe();
    //myNumber is 50
    command.Execute(25).Subscribe();
    //myNumber is 25
    command.History.Undo.Execute().Subscribe();
    //myNumber is 0
    command.History.Undo.Execute().Subscribe();
    //myNumber is 25
    command.History.Redo.Execute().Subscribe();
     //myNumber is 50
    command.History.Redo.Execute().Subscribe();
                       
 ```
 
 



## License📑

Copyright (c) SimpleStateMachine

Licensed under the [MIT](LICENSE) license.
