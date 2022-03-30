import { Room, Client } from "colyseus";
import { MyRoomState, NetworkedUser, NetworkedEntity } from "./schema/MyRoomState";

export class MyRoom extends Room<MyRoomState> {

  onCreate (options: any) {
    this.setState(new MyRoomState());

    this.onMessage("type", (client, message) => {
      //
      // handle "type" message
      //
    });

  }

  onJoin (client: Client, options: any) {
    console.log(client.sessionId, "joined!");
       
    let newNetworkedUser = new NetworkedUser().assign({
        sessionId: client.sessionId,
    });
    
    this.state.networkedUsers.set(client.sessionId, newNetworkedUser);
  }

  onLeave (client: Client, consented: boolean) {
    console.log(client.sessionId, "left!");
    let networkedUser = this.state.networkedUsers.get(client.sessionId);
    
    if(networkedUser){
        networkedUser.connected = false;
    }
  }

  onDispose() {
    console.log("room", this.roomId, "disposing...");
  }

}
