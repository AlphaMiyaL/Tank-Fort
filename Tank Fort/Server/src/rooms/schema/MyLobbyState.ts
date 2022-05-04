import { Schema, MapSchema, Context, type } from "@colyseus/schema";

export class LobbyUser extends Schema {
    @type("string") sessionId: string;
    @type("number") timestamp: number;
    @type("string") nickname: string;
    @type({map: "string"}) attributes = new MapSchema<string>();
}

export class MyLobbyState extends Schema {
    @type({ map: LobbyUser }) networkedUsers = new MapSchema<LobbyUser>();
}
