﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client.CheckersServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserAlreadyLoginFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    [System.SerializableAttribute()]
    public partial class UserAlreadyLoginFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usrNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string usrName {
            get {
                return this.usrNameField;
            }
            set {
                if ((object.ReferenceEquals(this.usrNameField, value) != true)) {
                    this.usrNameField = value;
                    this.RaisePropertyChanged("usrName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserNotExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    [System.SerializableAttribute()]
    public partial class UserNotExistsFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usrNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string usrName {
            get {
                return this.usrNameField;
            }
            set {
                if ((object.ReferenceEquals(this.usrNameField, value) != true)) {
                    this.usrNameField = value;
                    this.RaisePropertyChanged("usrName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="WrongPasswordFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    [System.SerializableAttribute()]
    public partial class WrongPasswordFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string usrNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string usrName {
            get {
                return this.usrNameField;
            }
            set {
                if ((object.ReferenceEquals(this.usrNameField, value) != true)) {
                    this.usrNameField = value;
                    this.RaisePropertyChanged("usrName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserNameAlreadyExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    [System.SerializableAttribute()]
    public partial class UserNameAlreadyExistsFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserNameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserName {
            get {
                return this.UserNameField;
            }
            set {
                if ((object.ReferenceEquals(this.UserNameField, value) != true)) {
                    this.UserNameField = value;
                    this.RaisePropertyChanged("UserName");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Status", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    public enum Status : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unfinished = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OneWon = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        TwoWon = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        isTie = 3,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GameIdNotExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    [System.SerializableAttribute()]
    public partial class GameIdNotExistsFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int gameIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int gameId {
            get {
                return this.gameIdField;
            }
            set {
                if ((this.gameIdField.Equals(value) != true)) {
                    this.gameIdField = value;
                    this.RaisePropertyChanged("gameId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Mode", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    public enum Mode : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lobby = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Watching = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Playing = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Result", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
    public enum Result : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Win = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Lost = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Tie = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Continue = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CheckersServiceReference.ICheckersService", CallbackContract=typeof(Client.CheckersServiceReference.ICheckersServiceCallback))]
    public interface ICheckersService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Connect", ReplyAction="http://tempuri.org/ICheckersService/ConnectResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Client.CheckersServiceReference.UserAlreadyLoginFault), Action="http://tempuri.org/ICheckersService/ConnectUserAlreadyLoginFaultFault", Name="UserAlreadyLoginFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
        [System.ServiceModel.FaultContractAttribute(typeof(Client.CheckersServiceReference.UserNotExistsFault), Action="http://tempuri.org/ICheckersService/ConnectUserNotExistsFaultFault", Name="UserNotExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
        [System.ServiceModel.FaultContractAttribute(typeof(Client.CheckersServiceReference.WrongPasswordFault), Action="http://tempuri.org/ICheckersService/ConnectWrongPasswordFaultFault", Name="WrongPasswordFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
        void Connect(string usrName, string hashedPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Connect", ReplyAction="http://tempuri.org/ICheckersService/ConnectResponse")]
        System.Threading.Tasks.Task ConnectAsync(string usrName, string hashedPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Register", ReplyAction="http://tempuri.org/ICheckersService/RegisterResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Client.CheckersServiceReference.UserNameAlreadyExistsFault), Action="http://tempuri.org/ICheckersService/RegisterUserNameAlreadyExistsFaultFault", Name="UserNameAlreadyExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
        void Register(string userName, string hashedPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Register", ReplyAction="http://tempuri.org/ICheckersService/RegisterResponse")]
        System.Threading.Tasks.Task RegisterAsync(string userName, string hashedPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetGame", ReplyAction="http://tempuri.org/ICheckersService/GetGameResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Client.CheckersServiceReference.GameIdNotExistsFault), Action="http://tempuri.org/ICheckersService/GetGameGameIdNotExistsFaultFault", Name="GameIdNotExistsFault", Namespace="http://schemas.datacontract.org/2004/07/CheckersService")]
        System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string> GetGame(int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetGame", ReplyAction="http://tempuri.org/ICheckersService/GetGameResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string>> GetGameAsync(int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Disconnect", ReplyAction="http://tempuri.org/ICheckersService/DisconnectResponse")]
        void Disconnect(string usrName, Client.CheckersServiceReference.Mode userMode, int numGame);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Disconnect", ReplyAction="http://tempuri.org/ICheckersService/DisconnectResponse")]
        System.Threading.Tasks.Task DisconnectAsync(string usrName, Client.CheckersServiceReference.Mode userMode, int numGame);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/MakeMove", ReplyAction="http://tempuri.org/ICheckersService/MakeMoveResponse")]
        void MakeMove(string UserName, int GameId, System.DateTime time, System.Windows.Point correntPos, int indexPath, Client.CheckersServiceReference.Result result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/MakeMove", ReplyAction="http://tempuri.org/ICheckersService/MakeMoveResponse")]
        System.Threading.Tasks.Task MakeMoveAsync(string UserName, int GameId, System.DateTime time, System.Windows.Point correntPos, int indexPath, Client.CheckersServiceReference.Result result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/JoinGame", ReplyAction="http://tempuri.org/ICheckersService/JoinGameResponse")]
        System.ValueTuple<int, string, bool> JoinGame(string user, bool isVsCPU, int boardSize, bool EatMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/JoinGame", ReplyAction="http://tempuri.org/ICheckersService/JoinGameResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<int, string, bool>> JoinGameAsync(string user, bool isVsCPU, int boardSize, bool EatMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/WatchGame", ReplyAction="http://tempuri.org/ICheckersService/WatchGameResponse")]
        System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string> WatchGame(string usrName, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/WatchGame", ReplyAction="http://tempuri.org/ICheckersService/WatchGameResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string>> WatchGameAsync(string usrName, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/CloseUnFinishedGame", ReplyAction="http://tempuri.org/ICheckersService/CloseUnFinishedGameResponse")]
        void CloseUnFinishedGame(int GameId, string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/CloseUnFinishedGame", ReplyAction="http://tempuri.org/ICheckersService/CloseUnFinishedGameResponse")]
        System.Threading.Tasks.Task CloseUnFinishedGameAsync(int GameId, string UserName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/StopWatchGame", ReplyAction="http://tempuri.org/ICheckersService/StopWatchGameResponse")]
        void StopWatchGame(string usrName, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/StopWatchGame", ReplyAction="http://tempuri.org/ICheckersService/StopWatchGameResponse")]
        System.Threading.Tasks.Task StopWatchGameAsync(string usrName, int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetAllMoves", ReplyAction="http://tempuri.org/ICheckersService/GetAllMovesResponse")]
        System.Collections.Generic.List<System.ValueTuple<int, System.DateTime, System.ValueTuple<int, int>, int, string>> GetAllMoves(int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetAllMoves", ReplyAction="http://tempuri.org/ICheckersService/GetAllMovesResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, System.DateTime, System.ValueTuple<int, int>, int, string>>> GetAllMovesAsync(int gameId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Ping", ReplyAction="http://tempuri.org/ICheckersService/PingResponse")]
        bool Ping();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/Ping", ReplyAction="http://tempuri.org/ICheckersService/PingResponse")]
        System.Threading.Tasks.Task<bool> PingAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/StopWaitingGame", ReplyAction="http://tempuri.org/ICheckersService/StopWaitingGameResponse")]
        void StopWaitingGame(string UserName, int boardSize, int eatMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/StopWaitingGame", ReplyAction="http://tempuri.org/ICheckersService/StopWaitingGameResponse")]
        System.Threading.Tasks.Task StopWaitingGameAsync(string UserName, int boardSize, int eatMode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetPlayedGames", ReplyAction="http://tempuri.org/ICheckersService/GetPlayedGamesResponse")]
        System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>> GetPlayedGames(string usrName1, string usrName2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetPlayedGames", ReplyAction="http://tempuri.org/ICheckersService/GetPlayedGamesResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>>> GetPlayedGamesAsync(string usrName1, string usrName2);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetPlayedGamesByDate", ReplyAction="http://tempuri.org/ICheckersService/GetPlayedGamesByDateResponse")]
        System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>> GetPlayedGamesByDate(System.DateTime date);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICheckersService/GetPlayedGamesByDate", ReplyAction="http://tempuri.org/ICheckersService/GetPlayedGamesByDateResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>>> GetPlayedGamesByDateAsync(System.DateTime date);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICheckersServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICheckersService/SendOpponentMove")]
        void SendOpponentMove(System.Windows.Point correntPos, int indexPath, Client.CheckersServiceReference.Result result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICheckersService/StartGame")]
        void StartGame(int GameId, string OpponentName, bool MyTurn);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICheckersService/CloseTheGame")]
        void CloseTheGame();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICheckersServiceChannel : Client.CheckersServiceReference.ICheckersService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CheckersServiceClient : System.ServiceModel.DuplexClientBase<Client.CheckersServiceReference.ICheckersService>, Client.CheckersServiceReference.ICheckersService {
        
        public CheckersServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public CheckersServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public CheckersServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CheckersServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CheckersServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Connect(string usrName, string hashedPassword) {
            base.Channel.Connect(usrName, hashedPassword);
        }
        
        public System.Threading.Tasks.Task ConnectAsync(string usrName, string hashedPassword) {
            return base.Channel.ConnectAsync(usrName, hashedPassword);
        }
        
        public void Register(string userName, string hashedPassword) {
            base.Channel.Register(userName, hashedPassword);
        }
        
        public System.Threading.Tasks.Task RegisterAsync(string userName, string hashedPassword) {
            return base.Channel.RegisterAsync(userName, hashedPassword);
        }
        
        public System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string> GetGame(int gameId) {
            return base.Channel.GetGame(gameId);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string>> GetGameAsync(int gameId) {
            return base.Channel.GetGameAsync(gameId);
        }
        
        public void Disconnect(string usrName, Client.CheckersServiceReference.Mode userMode, int numGame) {
            base.Channel.Disconnect(usrName, userMode, numGame);
        }
        
        public System.Threading.Tasks.Task DisconnectAsync(string usrName, Client.CheckersServiceReference.Mode userMode, int numGame) {
            return base.Channel.DisconnectAsync(usrName, userMode, numGame);
        }
        
        public void MakeMove(string UserName, int GameId, System.DateTime time, System.Windows.Point correntPos, int indexPath, Client.CheckersServiceReference.Result result) {
            base.Channel.MakeMove(UserName, GameId, time, correntPos, indexPath, result);
        }
        
        public System.Threading.Tasks.Task MakeMoveAsync(string UserName, int GameId, System.DateTime time, System.Windows.Point correntPos, int indexPath, Client.CheckersServiceReference.Result result) {
            return base.Channel.MakeMoveAsync(UserName, GameId, time, correntPos, indexPath, result);
        }
        
        public System.ValueTuple<int, string, bool> JoinGame(string user, bool isVsCPU, int boardSize, bool EatMode) {
            return base.Channel.JoinGame(user, isVsCPU, boardSize, EatMode);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<int, string, bool>> JoinGameAsync(string user, bool isVsCPU, int boardSize, bool EatMode) {
            return base.Channel.JoinGameAsync(user, isVsCPU, boardSize, EatMode);
        }
        
        public System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string> WatchGame(string usrName, int gameId) {
            return base.Channel.WatchGame(usrName, gameId);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<int, Client.CheckersServiceReference.Status, System.DateTime, bool, int, string, string>> WatchGameAsync(string usrName, int gameId) {
            return base.Channel.WatchGameAsync(usrName, gameId);
        }
        
        public void CloseUnFinishedGame(int GameId, string UserName) {
            base.Channel.CloseUnFinishedGame(GameId, UserName);
        }
        
        public System.Threading.Tasks.Task CloseUnFinishedGameAsync(int GameId, string UserName) {
            return base.Channel.CloseUnFinishedGameAsync(GameId, UserName);
        }
        
        public void StopWatchGame(string usrName, int gameId) {
            base.Channel.StopWatchGame(usrName, gameId);
        }
        
        public System.Threading.Tasks.Task StopWatchGameAsync(string usrName, int gameId) {
            return base.Channel.StopWatchGameAsync(usrName, gameId);
        }
        
        public System.Collections.Generic.List<System.ValueTuple<int, System.DateTime, System.ValueTuple<int, int>, int, string>> GetAllMoves(int gameId) {
            return base.Channel.GetAllMoves(gameId);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, System.DateTime, System.ValueTuple<int, int>, int, string>>> GetAllMovesAsync(int gameId) {
            return base.Channel.GetAllMovesAsync(gameId);
        }
        
        public bool Ping() {
            return base.Channel.Ping();
        }
        
        public System.Threading.Tasks.Task<bool> PingAsync() {
            return base.Channel.PingAsync();
        }
        
        public void StopWaitingGame(string UserName, int boardSize, int eatMode) {
            base.Channel.StopWaitingGame(UserName, boardSize, eatMode);
        }
        
        public System.Threading.Tasks.Task StopWaitingGameAsync(string UserName, int boardSize, int eatMode) {
            return base.Channel.StopWaitingGameAsync(UserName, boardSize, eatMode);
        }
        
        public System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>> GetPlayedGames(string usrName1, string usrName2) {
            return base.Channel.GetPlayedGames(usrName1, usrName2);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>>> GetPlayedGamesAsync(string usrName1, string usrName2) {
            return base.Channel.GetPlayedGamesAsync(usrName1, usrName2);
        }
        
        public System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>> GetPlayedGamesByDate(System.DateTime date) {
            return base.Channel.GetPlayedGamesByDate(date);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<System.ValueTuple<int, string, string, Client.CheckersServiceReference.Status, System.DateTime>>> GetPlayedGamesByDateAsync(System.DateTime date) {
            return base.Channel.GetPlayedGamesByDateAsync(date);
        }
    }
}
