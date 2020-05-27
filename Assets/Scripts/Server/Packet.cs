using UnityEngine;
using System.Collections;
using System.IO;

public class PacketWriter : BinaryWriter {
	private readonly MemoryStream memoryStream;

	public PacketWriter() : base() {
		memoryStream = new MemoryStream();
		OutStream = memoryStream;
	}

	public void Write(Vector3 vector) {
		Write(vector.x);
		Write(vector.y);
		Write(vector.z);
	}

	public void Write(Vector2 vector) {
		Write(vector.x);
		Write(vector.y);
	}

	public void Write(Quaternion quaternion) {
		Write(quaternion.x);
		Write(quaternion.y);
		Write(quaternion.z);
		Write(quaternion.w);
	}

	public byte[] GetBytes() {
		Close();
		byte[] data = memoryStream.ToArray();

		return data;
	}
}
public class PacketReader : BinaryReader {
	public PacketReader(byte[] data) : base(new MemoryStream(data)) { }

	public Vector3 ReadVector3() {
		return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
	}

	public Vector2 ReadVector2() {
		return new Vector2(ReadSingle(), ReadSingle());
	}

	public Quaternion ReadQuaternion() {
		return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
	}
}
