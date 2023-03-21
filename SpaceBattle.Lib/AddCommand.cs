namespace SpaceBattle.Lib
{    
    public class AddCommand : ICommand
    {
        ICommand command{ get; }
        Queue<ICommand> queue{ get; }
        public AddCommand(ICommand command, Queue<ICommand> queue)
        {
            this.command = command;
            this.queue = queue;
        }

        public void Execute()
        {
            queue.Enqueue(command);
            queue.Enqueue(this);
        }
    }
}
