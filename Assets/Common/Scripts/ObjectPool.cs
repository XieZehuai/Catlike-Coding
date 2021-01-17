/*
 * Author: Huai
 * Create: 2021/1/17 21:11:03
 *
 * Description:
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
	public class ObjectPool<T> where T : class, new()
	{
		private static readonly Stack<T> objectStack = new Stack<T>();

		public static T New()
		{
			return objectStack.Count == 0 ? new T() : objectStack.Pop();
		}

		public static void Store(T obj)
		{
			objectStack.Push(obj);
		}
	}
}
