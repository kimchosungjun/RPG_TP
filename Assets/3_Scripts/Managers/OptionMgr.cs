using UnityEngine;
using System.IO;

public partial class SaveMgr : MonoBehaviour
{
    #region Option Variable
    string optionFilePath = "OPTION.txt";
    public OptionData Option = new OptionData();
    public struct OptionData
    {
        public int masterVolume;
        public int bgmVolume;
        public int sfxVolume;
        public int graphicQuality;

        public void SetData(int[] _datas)
        {
            masterVolume = _datas[0];
            bgmVolume = _datas[1];
            sfxVolume = _datas[2];
            graphicQuality = _datas[3];
        }
    }
    #endregion

    public void LoadOptionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, optionFilePath);
        if (File.Exists(path))
        {
            StreamReader streamReader = File.OpenText(path);
            int[] optionValues = new int[4];
            int curIndex = 0;
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                optionValues[curIndex++] = int.Parse(line);
            }
            Option.SetData(optionValues);
            streamReader.Close();
        }
        else
        {
            StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine("1");
            streamWriter.WriteLine("1");
            streamWriter.WriteLine("1");
            streamWriter.WriteLine("2");
            streamWriter.Close();
        }
    }

    public void SaveOptionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, optionFilePath);
        StreamWriter streamWriter = File.CreateText(path);
        streamWriter.WriteLine(Option.masterVolume);
        streamWriter.WriteLine(Option.bgmVolume);
        streamWriter.WriteLine(Option.sfxVolume);
        streamWriter.WriteLine(Option.graphicQuality);
        streamWriter.Close();
    }
}
