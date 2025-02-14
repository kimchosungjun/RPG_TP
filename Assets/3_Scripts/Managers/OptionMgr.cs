using UnityEngine;
using System.IO;

public partial class SaveMgr : MonoBehaviour
{
    #region Option Variable
    string optionFilePath = "OPTION.txt";
    public OptionData Option = new OptionData();
    public struct OptionData
    {
        public float masterVolume;
        public float bgmVolume;
        public float sfxVolume;
        public int graphicQuality;

        public void SetData(float[] _volumes, int _quality)
        {
            masterVolume = _volumes[0];
            bgmVolume = _volumes[1];
            sfxVolume = _volumes[2];
            graphicQuality = _quality;
        }
    }
    #endregion

    public void LoadOptionFile()
    {
        string path = Path.Combine(Application.persistentDataPath, optionFilePath);
        if (File.Exists(path))
        {
            StreamReader streamReader = File.OpenText(path);
            float[] optionValues = new float[3];
            int curIndex = 0;
            string line;
            int qualityValue = 0;
            while ((line = streamReader.ReadLine()) != null)
            {
                if(curIndex<=2)
                    optionValues[curIndex] = float.Parse(line);
                else if (curIndex == 3)
                    qualityValue = int.Parse(line);
                curIndex += 1;
            }
            Option.SetData(optionValues, qualityValue);
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
