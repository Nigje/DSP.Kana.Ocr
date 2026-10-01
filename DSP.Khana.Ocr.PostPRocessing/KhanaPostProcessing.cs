using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP.Khana.Ocr.PostProcessing
{
    public class KhanaPostProcessing    
    {
        public string GetString(string inputString)
        {
            try
            {
                inputString = FilterSpaceSequenseRole(inputString);
                string[] splitedInput = inputString.Split(' ');
                List<string> finalResult = new List<string>(splitedInput);

                finalResult = FilterOneCharRole(finalResult);
                finalResult = FilterFakeWord(finalResult);

                string result = "";
                foreach (string str in finalResult)
                {
                    result += str + " ";
                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
        //*************************************************************************
        public List<string> FilterOneCharRole(List<string> inputString)
        {
          List<char>  trueChars =new List<char>() {'ا', 'و','~','`','!','@','#','$', '%', '^', '&', '*', '(', ')', '_', '-', '=', '+', ']', '}', '{', '[', '"', ';', ':', '/', '?', '>', '.', '<', ',', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '\t' };
            trueChars.Add((char)1776);
            trueChars.Add((char)1777);
            trueChars.Add((char)1778);
            trueChars.Add((char)1779);
            trueChars.Add((char)1780);
            trueChars.Add((char)1781);
            trueChars.Add((char)1782);
            trueChars.Add((char)1783);
            trueChars.Add((char)1784);
            trueChars.Add((char)1785);
            List<string> output=new List<string>();
            foreach (string str in inputString)
            {
                bool flag = false;
                if (str.Length == 1)
                {
                    foreach (char trueChar in trueChars)
                    {
                        if (Equals(str, trueChar))
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    flag = true;
                }
                if (flag)
                {
                    output.Add(str);
                }
            }
            return output;
        }
        //*************************************************************************
        public string FilterExtraEnter(string text)
        {
            while (text.Contains("\n\n"))
                text = text.Replace("\n\n", "\n");
            text = text.Replace(".\n", ".\n\n");
            return text;
        }
        //*************************************************************************
        public string FilterSpaceSequenseRole(string inputString)
        {
            while (inputString.Contains("  "))
                inputString = inputString.Replace("  ", " ");
            return inputString;
        }

        //*************************************************************************
        public List<string> FilterFakeWord(List<string> inputString)
        {
            bool lastStringIsTrueWord = true;
            List<string> result=new List<string>();
            foreach (string str in inputString)
            {
                if (IsTrueWord(str))
                {
                    result.Add(str);
                    lastStringIsTrueWord = true;
                }
                
                else
                {
                    if (lastStringIsTrueWord)
                    {
                        result.Add(str);
                    }
                    lastStringIsTrueWord = false;
                }
            }
            return result;
        }
        //*************************************************************************
        public bool IsTrueWord(string inputString, double Threshold=0.3)
        {
            List<char> fakeChars =new List<char>() { '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '-', '=', '+', ']', '}', '{', '[', '"', ';', ':', '/', '?', '>', '.', '<', ',', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '\t' };
            fakeChars.Add((char)1776);
            fakeChars.Add((char)1777);
            fakeChars.Add((char)1778);
            fakeChars.Add((char)1779);
            fakeChars.Add((char)1780);
            fakeChars.Add((char)1781);
            fakeChars.Add((char)1782);
            fakeChars.Add((char)1783);
            fakeChars.Add((char)1784);
            fakeChars.Add((char)1785);

            int cout = (int)(Threshold * inputString.Length)+1;
            int counter = 0;
            List<char> chars =new List<char>(inputString.ToCharArray());
            foreach (char ch in chars)
            {
                foreach (char fCh in fakeChars)
                {
                    if(ch.Equals(fCh))
                        counter++;
                }
                if (counter >= cout)
                    return false;
            }
            return true;
        }
    }
}
