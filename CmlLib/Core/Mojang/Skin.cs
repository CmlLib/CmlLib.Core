using System;

namespace CmlLib.Core.Mojang
{
    public enum SkinType { Steve, Alex }

    public class Skin
    {
        public Skin()
        {

        }

        public Skin(string url, SkinType type)
        {
            this.Url = url;
            this.Model = type;
        }

        public Skin(string url, string type)
        {
            this.Url = url;

            string lowerType = type?.ToLower();
            if (lowerType == "alex" || lowerType == "slim")
                Model = SkinType.Alex;
            else
                Model = SkinType.Steve;
        }

        public static SkinType GetDefaultSkinType(string userUUID)
        {
            int hex2int(char input)
            {
                return Convert.ToInt32(input.ToString(), 16);
            }

            int lsbsEven =
                hex2int(userUUID[7]) ^
                hex2int(userUUID[15]) ^
                hex2int(userUUID[23]) ^
                hex2int(userUUID[31]);

            return (lsbsEven == 0) ? SkinType.Steve : SkinType.Alex;
        }

        public string Url { get; set; }
        public SkinType Model { get; set; }
    }
}
