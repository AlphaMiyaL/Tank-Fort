import { Room, Client } from "colyseus";
import { MyRoomState, NetworkedUser, NetworkedEntity } from "./schema/MyRoomState";

export class MyRoom extends Room<MyRoomState> {

  onCreate (options: any) {
    this.setState(new MyRoomState());
    this.setPatchRate(20);

    this.onMessage("move", (client, message) => {
      if (message.init){
        console.log("sending initialization broadcast...")
        this.broadcast("player-initialized");
      }
      let newNetworkedEntity = new NetworkedEntity().assign({
        isPlayer: true,
        ownerId: client.sessionId,
        xPos: message.xPos,
        yPos: message.yPos,
        zPos: message.zPos,

        xRot: message.xRot,
        yRot: message.yRot,
        zRot: message.zRot,
        wRot: message.wRot,
      });
      this.state.networkedEntities.set(client.sessionId, newNetworkedEntity);
    })
    this.onMessage("fire", (client, message) => {
      this.broadcast("fire", {
        id: client.sessionId,
        force: message
      }, {
        except: client
      });
    })
    this.onMessage("self-dmg", (client, message) => {
      this.broadcast("dmg", {
        id: client.sessionId,
        dmg: message
      }, {
        except: client
      })
    })
  }

  onJoin (client: Client, options: any) {
    console.log(client.sessionId, "joined!");
    console.log(options);
    let newNetworkedUser = new NetworkedUser().assign({
        sessionId: client.sessionId,
        nickname: options.nickname,
    });
    
    this.state.networkedUsers.set(client.sessionId, newNetworkedUser);
  }

  onLeave (client: Client, consented: boolean) {
    console.log(client.sessionId, "left the room!");
    let networkedUser = this.state.networkedUsers.get(client.sessionId);
    
    if(networkedUser){
        networkedUser.connected = false;
    }
  }

  onDispose() {
    console.log("room", this.roomId, "disposing...");
  }

}
