using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlayerDictionary : SDictionary<int, Player> { }

[Serializable]
public class ConnectionDictionary : SDictionary<int, UserConnection> { }   

[Serializable]
public class SDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver {

    [SerializeField]
    private List<K> m_Keys;
    [SerializeField]
    private List<V> m_Values;

    static SDictionary() {
        if(!typeof(K).IsSerializable || !typeof(V).IsSerializable)
            Debug.LogError($"SDictionary: Types need to be serializable. K: {typeof(K).IsSerializable} V: {typeof(V).IsSerializable}");
    }

    public void OnBeforeSerialize() {
        m_Keys = Keys.ToList();
        m_Values = Values.ToList();
    }

    public void OnAfterDeserialize() {
        Clear();
        for(int idx = 0; idx < m_Keys.Count; ++idx)
            Add(m_Keys[idx], m_Values[idx]);
        m_Keys = null;
        m_Values = null;
    }
}