using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackCoat
{
    public class TextEnteredEventArgs
    {
        public String Text { get; set; }
        public Boolean Backspace { get; set; }

        public TextEnteredEventArgs(String text)
        {
            Text = text;
        }

        public TextEnteredEventArgs(Boolean backSpace)
        {
            Backspace = backSpace;
        }

        public String UpdateText(String input)
        {
            if (input == null) input = String.Empty;

            if (Backspace)
            {
                if (input.Length != 0)
                {
                    input = input.Remove(input.Length - 1, 1);
                }
            }
            else
            {
                input = String.Concat(input, Text);
            }
            return input;
        }
    }
}