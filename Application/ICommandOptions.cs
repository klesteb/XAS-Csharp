using System;

namespace XAS.App {

    public interface ICommandOptions {

        String Prompt { get; set; }

        Int32 Process(params String[] args);
        CommandOptions Add(string name, string description, Func<String[], Boolean> action);

    }

}
