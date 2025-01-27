namespace AMTServerDLL.Dto
{
    public class ResponseTutorialAddArtifacts : SendData
    {
        public int Artifacts;

        public ResponseTutorialAddArtifacts(int artifacts)
        {
            Artifacts = artifacts;
        }
    }
}