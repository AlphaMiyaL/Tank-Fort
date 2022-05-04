// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.34
// 

using Colyseus.Schema;

public partial class LobbyUser : Schema {
	[Type(0, "string")]
	public string sessionId = default(string);

	[Type(1, "number")]
	public float timestamp = default(float);

	[Type(2, "string")]
	public string nickname = default(string);

	[Type(3, "map", typeof(MapSchema<string>), "string")]
	public MapSchema<string> attributes = new MapSchema<string>();
}

