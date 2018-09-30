namespace Storiveo.IsisPie.EventArgs
{
	public class SendToPumpControllerEventArgs : System.EventArgs
    {
		public byte[] Data;

		public SendToPumpControllerEventArgs(byte[] data)
        {
			Data = new byte[data.Length];
			Data = data;
        }
    }
}