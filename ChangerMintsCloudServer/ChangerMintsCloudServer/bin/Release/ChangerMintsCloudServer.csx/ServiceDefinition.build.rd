<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="ChangerMintsCloudServer" generation="1" functional="0" release="0" Id="643114aa-678e-4219-8f90-338b61aa656b" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="ChangerMintsCloudServerGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="ChangerMintsWorkerRole:ChangerMintsEndpoint" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/LB:ChangerMintsWorkerRole:ChangerMintsEndpoint" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="ChangerMintsWorkerRole:?IsSimulationEnvironment?" defaultValue="">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRole:?IsSimulationEnvironment?" />
          </maps>
        </aCS>
        <aCS name="ChangerMintsWorkerRole:?RoleHostDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRole:?RoleHostDebugger?" />
          </maps>
        </aCS>
        <aCS name="ChangerMintsWorkerRole:?StartupTaskDebugger?" defaultValue="">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRole:?StartupTaskDebugger?" />
          </maps>
        </aCS>
        <aCS name="ChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="ChangerMintsWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/MapChangerMintsWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:ChangerMintsWorkerRole:ChangerMintsEndpoint">
          <toPorts>
            <inPortMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/ChangerMintsEndpoint" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapChangerMintsWorkerRole:?IsSimulationEnvironment?" kind="Identity">
          <setting>
            <aCSMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/?IsSimulationEnvironment?" />
          </setting>
        </map>
        <map name="MapChangerMintsWorkerRole:?RoleHostDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/?RoleHostDebugger?" />
          </setting>
        </map>
        <map name="MapChangerMintsWorkerRole:?StartupTaskDebugger?" kind="Identity">
          <setting>
            <aCSMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/?StartupTaskDebugger?" />
          </setting>
        </map>
        <map name="MapChangerMintsWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapChangerMintsWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="ChangerMintsWorkerRole" generation="1" functional="0" release="0" software="D:\CM\ChangerMintsCloudServer\ChangerMintsCloudServer\bin\Release\ChangerMintsCloudServer.csx\roles\ChangerMintsWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="ChangerMintsEndpoint" protocol="tcp" portRanges="4545" />
            </componentports>
            <settings>
              <aCS name="?IsSimulationEnvironment?" defaultValue="" />
              <aCS name="?RoleHostDebugger?" defaultValue="" />
              <aCS name="?StartupTaskDebugger?" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;ChangerMintsWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;ChangerMintsWorkerRole&quot;&gt;&lt;e name=&quot;ChangerMintsEndpoint&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRoleInstances" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyID name="ChangerMintsWorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="69ebe77b-a5ec-4a8a-a602-0999b227acac" ref="Microsoft.RedDog.Contract\ServiceContract\ChangerMintsCloudServerContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="fb943e82-92d8-472b-b946-db747f052240" ref="Microsoft.RedDog.Contract\Interface\ChangerMintsWorkerRole:ChangerMintsEndpoint@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/ChangerMintsCloudServer/ChangerMintsCloudServerGroup/ChangerMintsWorkerRole:ChangerMintsEndpoint" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>