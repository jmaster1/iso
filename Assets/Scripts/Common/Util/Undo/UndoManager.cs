using System.Collections.Generic;
using Common.IO.Streams;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util.Http;

namespace Common.Util.Undo
{
    /// <summary>
    /// undo manager maintains list of commands that might be applied/reverted
    /// </summary>
    public class UndoManager : GenericBean
    {
        /// <summary>
        /// list of commands
        /// </summary>
        public readonly List<IUndoableCommand> commands = new List<IUndoableCommand>(16);

        public int Size => commands.Count;
        
        /// <summary>
        /// points to last added/reverted command
        /// </summary>
        public int cursor;

        public readonly BoolHolder undoAvailable = new BoolHolder();
        
        public bool IsUndoAvailable => cursor > 0 && CurrentCommand == null;
        
        public bool IsRedoAvailable => cursor < Size && CurrentCommand == null;
        
        public readonly BoolHolder redoAvailable = new BoolHolder();

        /// <summary>
        /// current command being executed
        /// </summary>
        public IUndoableCommand CurrentCommand { get; private set; }
        
        public override void Clear()
        {
            commands.Clear();
            cursor = 0;
            CurrentCommand = null;
            Update();
        }
        
        private void Update()
        {
            undoAvailable.Set(IsUndoAvailable);
            redoAvailable.Set(IsRedoAvailable);
        }

        public void AddCommand(IUndoableCommand cmd, bool apply = true)
        {
            if (cursor < Size)
            {
                commands.RemoveRange(cursor, Size - cursor);
            }
            commands.Add(cmd);
            cursor++;
            Validate(cursor == Size);
            Update();
            if (!apply) return;
            CurrentCommand = cmd;
            cmd.Apply(OnComplete);
        }

        private void OnComplete()
        {
            CurrentCommand = null;
            Update();
        }

        public void Undo()
        {
            Validate(IsUndoAvailable);
            CurrentCommand = commands[cursor - 1];
            cursor--;
            Update();
            CurrentCommand.Revert(OnComplete);
        }
        
        public void Redo()
        {
            Validate(IsRedoAvailable);
            CurrentCommand = commands[cursor];
            cursor++;
            Update();
            CurrentCommand.Apply(OnComplete);
        }

        public override void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
            html.propertyTable("cursor", cursor,
                "CurrentCommand", CurrentCommand);
            html.tableHeader("#", "cmd");
            foreach (var cmd in commands)
            {
                html.tr().tdRowNum().td(cmd).endTr();
            }

            html.endTable();
        }
    }
}
