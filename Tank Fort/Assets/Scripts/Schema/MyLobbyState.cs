// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.34
// 

using Colyseus.Schema;

public partial class MyLobbyState : Schema {
	[Type(0, "map", typeof(MapSchema<LobbyUser>))]
	public MapSchema<LobbyUser> networkedUsers = new MapSchema<LobbyUser>();
}

