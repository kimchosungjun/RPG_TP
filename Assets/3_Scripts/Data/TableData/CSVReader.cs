using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CSVReader
{
    //	private UnityEngine.TextAsset _csv_file;
    private System.String[,] _arr_grid;

    public CSVReader()
    {
    }

    public CSVReader(System.String[,] grid)
    {
        _arr_grid = grid;
    }

    public System.String[,] grid
    {
        get { return _arr_grid; }
    }

    public CSVReader parse(UnityEngine.TextAsset text_asset, bool debug)
    {
        //		CDebug.Log( "", text_asset );
        parse(text_asset.text, debug);
        //		CDebug.Log( "size = " + ( 1+ _arr_grid.GetUpperBound( 0 ) ) + "," + ( 1 + _arr_grid.GetUpperBound( 1 ) ) ); 

        return this;
    }

    public CSVReader parse(string text, bool debug, int nEncode = 0)
    {
        //		CDebug.Log( "", text_asset );
        _arr_grid = _split_csv_grid(text, nEncode);
        //		CDebug.Log( "size = " + ( 1+ _arr_grid.GetUpperBound( 0 ) ) + "," + ( 1 + _arr_grid.GetUpperBound( 1 ) ) ); 

        if (debug)
            debug_output_grid();

        return this;
    }

    // outputs the content of a 2D array, useful for checking the importer
    public void debug_output_grid()
    {
        System.String textOutput = "";
        for (int y = 0; y < _arr_grid.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _arr_grid.GetUpperBound(0); x++)
            {

                textOutput += _arr_grid[x, y];
                textOutput += "|";
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    public System.String[] get_row_array(int row)
    {
        System.String[] arr = new System.String[column];
        for (int i = 0; i < column; ++i)
        {
            arr[i] = _arr_grid[i, row];
        }
        return arr;
    }

    public bool is_data(int row, int col)
    {
        string s = _arr_grid[col, row];

        if ((s == null) || (s == ""))
            return false;

        return true;
    }

    public int get_int(int row, int col)
    {
        string s = _arr_grid[col, row];

        if ((s == null) || (s == ""))
            return 0;

        return (int)System.Convert.ToInt32(s);
    }

    private int cur_col = 0;
    public bool reset_row(int row, int StartCol)
    {
        cur_col = StartCol;
        string s = _arr_grid[StartCol, row];
        if (s == null)
            return false;

        if (_arr_grid[StartCol, row] == "")
            return false;

        return true;
    }

    public void get(int row, ref bool val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = false;
            return;
        }

        val = ((int)System.Convert.ToInt32(s) != 0);
    }

    public void get(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = (int)System.Convert.ToInt32(s);
    }

    public void get(int row, ref byte val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = (byte)System.Convert.ToInt32(s);
    }

    public void getlong(int row, ref long val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = (long)System.Convert.ToInt64(s);
    }

    public void get(int row, ref int[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = 0;
                continue;
            }

            val[i] = (int)System.Convert.ToInt32(s);
        }
    }

    public void get(int row, ref float val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0f;
            return;
        }

        val = (float)System.Convert.ToSingle(s);
    }

    public void get(int row, ref float[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = 0f;
                continue;
            }

            val[i] = (float)System.Convert.ToSingle(s);
        }
    }

    public void get(int row, ref string val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = "";
            return;
        }

        val = s;
    }

    public void get(int row, ref string[] val, int nCnt)
    {
        for (int i = 0; i < nCnt; ++i)
        {
            string s = _arr_grid[cur_col, row];
            ++cur_col;

            if ((s == null) || (s == ""))
            {
                val[i] = "";
                continue;
            }

            val[i] = s;
        }
    }

    public void get_Anihash(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = Animator.StringToHash(s);
    }

    public void get_hash(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        if ((s == null) || (s == ""))
        {
            val = 0;
            return;
        }

        val = s.GetHashCode();
    }

    public void get_bit(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        val = 0;
        if ((s == null) || (s == ""))
        {
            return;
        }


        int v = (int)System.Convert.ToInt32(s);
        int nBitIndex = 0;
        while (v != 0)
        {
            if (v % 10 == 1)
                val |= 1 << nBitIndex;

            v = v / 10;
            ++nBitIndex;
        }
    }

    public void get_bit_ZeroToAll(int row, ref int val)
    {
        string s = _arr_grid[cur_col, row];
        ++cur_col;

        val = (int)0xFFFF;
        if ((s == null) || (s == ""))
        {
            return;
        }

        int v = (int)System.Convert.ToInt32(s);
        if (v == 0)
            return;

        val = 0;
        int nBitIndex = 0;
        while (v != 0)
        {
            if (v % 10 == 1)
                val |= 1 << nBitIndex;

            v = v / 10;
            ++nBitIndex;
        }
    }

    public void next()
    {
        ++cur_col;
    }

    // search
    public CSVReader find(int field_index, System.String value)
    {
        System.Collections.Generic.List<int> list_index = new System.Collections.Generic.List<int>();
        for (int i = 0; i < _arr_grid.GetUpperBound(1); ++i)
        {
            //			CDebug.Log( "dance", _arr_grid[field_index,i], value, value != _arr_grid[field_index,i] );
            if (value != _arr_grid[field_index, i])
                continue;

            list_index.Add(i);
            //			CDebug.Log( "found :", i );
        }

        if (0 == list_index.Count)
            return null;

        System.String[,] arr_new_grid = new System.String[_arr_grid.GetUpperBound(0) + 1, list_index.Count + 1];
        for (int i = 0; i < _arr_grid.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < list_index.Count; ++j)
            {
                arr_new_grid[i, j] = _arr_grid[i, list_index[j]];
            }
        }
        //		CDebug.Log( "dance", value, arr_new_grid.Length );
        return new CSVReader(arr_new_grid);
    }

    //--------------------
    // search easy table value
    //--------------------
    public System.String find_value(int field_index, System.String value, System.Object field)
    {
        return find(field_index, value).grid[System.Convert.ToInt32(field), 0];
    }

    public int column
    {
        get { return _arr_grid.GetUpperBound(0); }
    }

    public int row
    {
        get { return _arr_grid.GetUpperBound(1); }
    }

    // splits a CSV file into a 2D string array
    private System.String[,] _split_csv_grid(System.String csvText, int nEncode)
    {
        if (2 == nEncode)
            csvText = csvText.Replace("\t", ",");

        bool bFindNewLine = false;
        int nFindStartIndex = 0;
        int nFindEndIndex = 0;
        List<string> strList = new List<string>();
        for (int i = 0; i < csvText.Length; ++i)
        {
            if (csvText[i] == '"')
            {
                if (bFindNewLine == false)
                {
                    nFindStartIndex = i;
                    strList.Add(csvText.Substring(nFindEndIndex, nFindStartIndex - nFindEndIndex));
                    bFindNewLine = true;
                }
                else if (bFindNewLine == true)
                {
                    nFindEndIndex = i + 1;
                    string parcing = csvText.Substring(nFindStartIndex, nFindEndIndex - nFindStartIndex);
                    parcing = parcing.Replace("\"", "");
                    parcing = parcing.Replace("\n", "\\z");
                    strList.Add(parcing);
                    bFindNewLine = false;
                }
            }
        }


        if (strList.Count > 0)
        {
            strList.Add(csvText.Substring(nFindEndIndex, csvText.Length - 1 - nFindEndIndex));

            csvText = "";
            for (int i = 0; i < strList.Count; ++i)
            {
                csvText += strList[i];
            }
        }

        System.String[] lines = csvText.Split("\n"[0]);



        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            System.String[] row = _split_csv_line(lines[i]);
            width = UnityEngine.Mathf.Max(width, row.Length);
        }

        // creates new 2D string grid to output to
        System.String[,] outputGrid = new System.String[width + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            lines[y] = lines[y].Replace("/,", "asdf!@#$");
            System.String[] row = _split_csv_line(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                row[x] = row[x].Replace("asdf!@#$", ",");

                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace(@"\n", "\n");
                outputGrid[x, y] = outputGrid[x, y].Replace(@"\z", "\n");
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }

        return outputGrid;
    }

    // splits a CSV row 
    private System.String[] _split_csv_line(System.String line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
           @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
           System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}