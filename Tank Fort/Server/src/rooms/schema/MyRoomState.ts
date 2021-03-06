import { Schema, MapSchema, Context, type } from "@colyseus/schema";

export class NetworkedEntity extends Schema {
  @type("boolean") isPlayer: boolean = false;
  @type("string") id: string;
  @type("string") ownerId: string;
  @type("number") xPos: number = 0;
  @type("number") yPos: number = 0;
  @type("number") zPos: number = 0;
  @type("number") xRot: number = 0;
  @type("number") yRot: number = 0;
  @type("number") zRot: number = 0;
  @type("number") wRot: number = 0;
  @type("number") xScale: number = 1;
  @type("number") yScale: number = 1;
  @type("number") zScale: number = 1;
  @type("number") xVel: number = 0;
  @type("number") yVel: number = 0;
  @type("number") zVel: number = 0;
  @type("number") timestamp: number;
  @type({map: "string"}) attributes = new MapSchema<string>();
}

export class NetworkedUser extends Schema {
  @type("string") sessionId: string;
  @type("boolean") connected: boolean;
  @type("number") timestamp: number;
  @type("string") nickname: string;
  @type({map: "string"}) attributes = new MapSchema<string>();
}

export class MyRoomState extends Schema {
  @type({ map: NetworkedEntity }) networkedEntities = new MapSchema<NetworkedEntity>();
  @type({ map: NetworkedUser }) networkedUsers = new MapSchema<NetworkedUser>();
  @type({ map: "string" }) attributes = new MapSchema<string>();
}
