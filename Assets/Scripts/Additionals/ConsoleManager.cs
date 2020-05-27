using UnityEngine;
using System.Collections;
using System;

public enum MessageType { Normal, Warning, Error }
public class ConsoleManager {
	public static void WriteDB(string text, MessageType type = MessageType.Normal) {
		#pragma warning disable CS0219 // Переменная назначена, но ее значение не используется
		ConsoleColor color = ConsoleColor.White;
		#pragma warning restore CS0219 // Переменная назначена, но ее значение не используется
		switch(type) {
			case MessageType.Normal:
				color = ConsoleColor.White;
				break;
			case MessageType.Warning:
				color = ConsoleColor.Yellow;
				break;
			case MessageType.Error:
				color = ConsoleColor.Red;
				break;
		}
		#if UNITY_EDITOR
		Debug.Log("--Database Message--");
		Debug.Log(text);
		#else
		Console.ForegroundColor = color;
		Console.WriteLine("--Database Message--");
		Console.WriteLine(text);
		Console.WriteLine();
		#endif
	}

	public static void WriteServer(string text, MessageType type = MessageType.Normal) {
		#pragma warning disable CS0219 // Переменная назначена, но ее значение не используется
		ConsoleColor color = ConsoleColor.White;
		#pragma warning restore CS0219 // Переменная назначена, но ее значение не используется
		switch(type) {
			case MessageType.Normal:
				color = ConsoleColor.White;
				break;
			case MessageType.Warning:
				color = ConsoleColor.Yellow;
				break;
			case MessageType.Error:
				color = ConsoleColor.Red;
				break;
		}

		#if UNITY_EDITOR
		Debug.Log("--Server Message--");
		Debug.Log(text);
		#else
		Console.ForegroundColor = color;
		Console.WriteLine("--Server Message--");
		Console.WriteLine(text);
		Console.WriteLine();
		#endif
	}
}
