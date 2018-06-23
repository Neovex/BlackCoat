using System;

namespace BlackCoat
{
    /// <summary>
    /// Contains information on text input and deletion
    /// </summary>
    public class TextEnteredEventArgs
    {
        /// <summary>
        /// Entered Text. Usually not more than one character at a time.
        /// </summary>
        public String Text { get; }
        /// <summary>
        /// Determines if a backspace input happened.
        /// </summary>
        public Boolean Backspace { get; }
        /// <summary>
        /// Determines if a delete input happened.
        /// </summary>
        public Boolean Del { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="TextEnteredEventArgs"/> class.
        /// </summary>
        /// <param name="text">The newly entered text</param>
        public TextEnteredEventArgs(String text)
        {
            Text = text;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextEnteredEventArgs"/> class.
        /// </summary>
        /// <param name="backSpace">Determines if a backspace input happened</param>
        /// <param name="del">Determines if a delete input happened</param>
        public TextEnteredEventArgs(Boolean backSpace, Boolean del)
        {
            Backspace = backSpace;
            Del = del;
        }


        /// <summary>
        /// DEPRECATED
        /// Updates a string based on the input represented by this <see cref="TextEnteredEventArgs"/> instance.
        /// </summary>
        /// <param name="original">The original string that should be modified</param>
        /// <param name="index">Current caret position</param>
        /// <returns>The modified string</returns>
        [Obsolete("This overload is deprecated, please use UpdateText(string, ref index) instead.")]
        public String UpdateText(String input)
        {
            int i = -1;
            return UpdateText(input, ref i);
        }
        /// <summary>
        /// Updates a string based on the input represented by this <see cref="TextEnteredEventArgs"/> instance.
        /// </summary>
        /// <param name="original">The original string that should be modified</param>
        /// <param name="index">Current caret position</param>
        /// <returns>The modified string</returns>
        public String UpdateText(String original, ref Int32 index)
        {
            // Sanity
            if (original == null)
            {
                original = String.Empty;
                index = 0;
            }
            else if (index < 0)
            {
                index = original.Length; // remove this block when deprecated UpdateText is removed
            }

            // Modify
            if (Backspace)
            {
                if (original.Length != 0)
                {
                    original = original.Remove(original.Length - 1, 1);
                }
            }
            else if (Del)
            {
                if (index != original.Length)
                {
                    original = original.Remove(index, 1);
                }
            }
            else
            {
                original = original.Insert(index, Text);
            }
            return original;
        }
    }
}