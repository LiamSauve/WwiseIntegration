//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class AkBankLoad : MonoBehaviour 
{
	public List<string> bankNames = new List<string>();	
	private uint[] m_BankIDs;

	void Start()
	{
		m_BankIDs = new uint[bankNames.Count];
		for (int i = 0; i < bankNames.Count; ++i)
		{
			AkSoundEngine.LoadBank(bankNames[i], AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankIDs[i]);
		}
	}
	
	void OnDisable()
	{		
		foreach(var bankIdIt in m_BankIDs)
		{
			IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
			AkSoundEngine.UnloadBank(bankIdIt, in_pInMemoryBankPtr);
		}
	}
}
