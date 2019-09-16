# SimpleResourceManager
Simple Resource Manager realised in Akka.NET Core

This Project consists of three parts:
1. Client side (responsible for creating new processes according to input message)
2. Server side (responsible for sharing jobs across nodes)
3. Messages, which is shared across Akka actors, in order to pass serialization

