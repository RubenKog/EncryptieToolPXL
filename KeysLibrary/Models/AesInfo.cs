namespace KeysLibrary.Models
{
	public class AesInfo
	{
		public string AesName { get; set; }
		public string AesKey { get; set; }
		public string AesIV { get; set; }

		public override string ToString() => AesName;
	}
}