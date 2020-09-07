using System;

namespace BlackCoat
{
    /// <summary>
    /// Contains information on text input, deletion and navigation
    /// </summary>
    public class TextEnteredEventArgs
    {
        /// <summary>
        /// Defines a type of navigation or modification to a string.
        /// </summary>
        public enum Operation
        {
            TextInput,
            Backspace,
            Del,
            Home,
            End,
            Left,
            Right,
        }


        /// <summary>
        /// Entered Text. Usually not more than one character at a time.
        /// </summary>
        public String Text { get; }

        /// <summary>
        /// Determines what kind of navigation or text modification occurred.
        /// </summary>
        public Operation Modification { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TextEnteredEventArgs"/> class.
        /// </summary>
        /// <param name="text">The newly entered text</param>
        public TextEnteredEventArgs(String text)
        {
            Text = text;
            Modification = Operation.TextInput;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEnteredEventArgs"/> class.
        /// </summary>
        /// <param name="operation">The operation that should modify the text.</param>
        public TextEnteredEventArgs(Operation operation)
        {
            Text = String.Empty;
            Modification = operation;
        }


        /// <summary>
        /// Updates a string based on the input represented by this <see cref="TextEnteredEventArgs"/> instance.
        /// </summary>
        /// <param name="original">The original string that should be modified</param>
        /// <param name="index">Current caret position</param>
        /// <returns>The modified string</returns>
        public String Update(String original, ref UInt32 index)
        {
            // Sanity
            if (original == null)
            {
                original = String.Empty;
                index = 0;
            }

            // Modify
            switch (Modification)
            {
                case Operation.TextInput:
                    original = original.Insert((int)index, Text);
                    index += (uint)Text.Length;
                    break;
                case Operation.Backspace:
                    if (index != 0)
                    {
                        index--;
                        original = original.Remove((int)index, 1);
                    }
                    break;
                case Operation.Del:
                    if (index != original.Length) original = original.Remove((int)index, 1);
                    break;
                case Operation.Home:
                    index = 0;
                    break;
                case Operation.End:
                    index = (uint)(original.Length);
                    break;
                case Operation.Left:
                    if(index != 0) index--;
                    break;
                case Operation.Right:
                    if (index != original.Length) index++;
                    break;
            }
            return original;
        }
    }
}