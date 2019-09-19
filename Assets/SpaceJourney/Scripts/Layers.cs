using UnityEngine;
using System.Collections;

namespace EazyEngine.Tools
{	
	public class Layers  
	{
		public static bool LayerInLayerMask(int layer, LayerMask layerMask)
		{
			if(((1 << layer) & layerMask) != 0)	
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
	}
}