namespace System.Drawing.Psd
{
	class LengthWriter : IDisposable
	{
        private long _lengthPosition = long.MinValue;
		private readonly long _startPosition;
		private readonly BinaryReverseWriter _reverseWriter;

		public LengthWriter(BinaryReverseWriter writer)
		{
			_reverseWriter = writer;

			// we will write the correct length later, so remember 
			// the position
			_lengthPosition = _reverseWriter.BaseStream.Position;
			_reverseWriter.Write(0xFEEDFEED);

			// remember the start  position for calculation Image 
			// resources length
			_startPosition = _reverseWriter.BaseStream.Position;
		}

		public void Write()
		{
			if (_lengthPosition == long.MinValue) return;

			long endPosition = _reverseWriter.BaseStream.Position;

			_reverseWriter.BaseStream.Position = _lengthPosition;
			long length = endPosition - _startPosition;
			_reverseWriter.Write((uint)length);
			_reverseWriter.BaseStream.Position = endPosition;

			_lengthPosition = long.MinValue;
		}

		public void Dispose()
		{
			Write();
		}
	}
}