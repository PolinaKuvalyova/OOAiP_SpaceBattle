namespace SpaceBattle.Lib
{
    public class MacroCommand : ICommand
    {
        public List<ICommand> commands { get; set; }
        public MacroCommand(List<ICommand>commands){
            this.commands = commands;
        }

        public void Execute()
        {
            foreach(ICommand command in commands)
            {
                command.Execute();
            }
        }
        
    }
}
