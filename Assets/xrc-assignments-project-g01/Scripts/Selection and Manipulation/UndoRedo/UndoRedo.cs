using System.Collections.Generic;
using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class UndoRedo : MonoBehaviour
    {
        public interface ICommand
        {
            void Execute();
            void Undo();
            void Redo();
            
            /// <summary>
            /// The command has been forgotten.
            /// Permanently apply the command.
            /// </summary>
            void ForgetAction();
        }

        [SerializeField] [Tooltip("The maximum amount of steps to remember. Set to -1 to have unlimited steps")]
        private int m_MemorySize = -1;
        
        private static UndoRedo _instance;
        public static UndoRedo Instance => _instance;
        
        private LinkedList<ICommand> m_UndoBuffer = new LinkedList<ICommand>();
        private LinkedList<ICommand> m_RedoBuffer = new LinkedList<ICommand>();
        
        private void Awake()
        {
            CreateInstance();
            DontDestroyOnLoad(gameObject); // keep active across scenes
        }

        private void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = this;
            } 
            else 
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Adds a command and executes it
        /// </summary>
        /// <param name="command">The added and executed command</param>
        public void AddCommand(ICommand command)
        {
            command.Execute();
            AddCapped(m_UndoBuffer, command);

            foreach (ICommand redoCommand in m_RedoBuffer)
            {
                redoCommand.ForgetAction();
            }
            m_RedoBuffer.Clear();
        }
        
        /// <summary>
        /// Undo a command
        /// </summary>
        public void Undo()
        {
            if(m_UndoBuffer.Count == 0)
                return;
    
            ICommand command = m_UndoBuffer.First.Value;
            m_UndoBuffer.RemoveFirst();
            command.Undo();

            AddCapped(m_RedoBuffer, command);
        }

        /// <summary>
        /// Redo a command
        /// </summary>
        public void Redo()
        {
            if(m_RedoBuffer.Count == 0)
                return;
    
            ICommand command = m_RedoBuffer.First.Value;
            m_RedoBuffer.RemoveFirst();
            command.Redo();

            AddCapped(m_UndoBuffer, command);
        }

        /// <summary>
        /// Adds a command.
        /// Handles maximum command size to remember
        /// </summary>
        /// <param name="list">The list to add a command to</param>
        /// <param name="command">The command to add</param>
        private void AddCapped(LinkedList<ICommand> list, ICommand command)
        {
            list.AddFirst(command);
            if (list.Count > m_MemorySize && m_MemorySize >= 0)
            {
                ICommand last = list.Last.Value;
                list.RemoveLast();
                last.ForgetAction();
            }
        }
    }
}