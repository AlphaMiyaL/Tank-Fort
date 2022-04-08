import { Room, Client } from "colyseus";
import { LobbyUser, MyLobbyState } from "./schema/MyLobbyState";

export class MyLobby extends Room<MyLobbyState> {

    onCreate (options: any) {
        this.setState(new MyLobbyState());
        this.setPatchRate(1000);

        this.onMessage("game-started", (client, message) => {
            this.broadcast("game-started", {
                roomId: message
            });
        })
    }

    onJoin (client: Client, options: any) {
        console.log(client.sessionId, "joined!");
        console.log(options);
        let newNetworkedUser = new LobbyUser().assign({
            sessionId: client.sessionId,
            nickname: options.nickname,
        });
        
        this.state.networkedUsers.set(client.sessionId, newNetworkedUser);
    }
  
    onLeave (client: Client, consented: boolean) {
        console.log(client.sessionId, "left the lobby!");
        
        this.state.networkedUsers.delete(client.sessionId);
    }
  
    onDispose() {
      console.log("lobby", this.roomId, "disposing...");
    }
}