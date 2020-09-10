using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Tests.ReportedIssues.Shared.Issue442;
using ExtendedXmlSerializer.Tests.ReportedIssues.Support;
using FluentAssertions;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue442Tests
	{
		[Fact]
		public void Verify()
		{
			const string content = @"<Device fb=""FB_Pneumatic2Pos"" commandStruct=""E_Pneumatic_Cmd"">
	  <Definition><![CDATA[	""#InstanceName#""(PlcCycleTime := ""DB_Time"".PlcCycleTime,
		OpMode := ""DB_#Station#_Station"".OpMode,
		Inactive := FALSE,
		I_SensorA := #I_SensorA#,
		I_SensorB := #I_SensorB#,
		C_TwoCoils := #C_TwoCoils#,
		C_ForceVentilate := #C_ForceVentilate#,
		C_Clamping := #C_Clamping#,
		C_PL := ""E_PL"".#C_PL#,
		C_TimeVentilate := T#200ms,
		C_TimeoutDuration := T#3s,
		C_WaitTimePosA := T#0ms,
		C_WaitTimePosB := T#0ms,
		Q_ValveA => #Q_ValveA#,
		Q_ValveB => #Q_ValveB#,
		Q_ValveOpenClamping => #Q_ValveOpenClamping#);]]></Definition>
	  <DeviceDataDb><![CDATA[]]></DeviceDataDb>
	  <ServiceHandler><![CDATA[	""FC_Pneumatic2Pos_Service_3Rows""(DeviceNo := #DeviceNo#,
		Row := #Row#,
		RowVisible := TRUE,
		DeviceName_Textlist := ""E_Service3Row_DeviceNames"".""#DeviceName#"",
		ButtonTextPosA_Textlist := ""E_Service_ButtonText"".""#ActionA#"",
		ButtonTextPosB_Textlist := ""E_Service_ButtonText"".""#ActionB#"",
		I_PosA_2 := #I_SensorA2#,
		I_PosB_2 := #I_SensorB2#,
		IDB := #InstanceName#,
		Btn_DeActivateService := ""DB_#Station#_Station"".Buttons.Service);]]></ServiceHandler>
	  <ParameterDb><![CDATA[]]></ParameterDb>
	  <Releases><![CDATA[		""#InstanceName#"".ReleasePosA := FALSE;
		""#InstanceName#"".ReleasePosB := FALSE;]]></Releases>
	  <CommandStart><![CDATA[]]></CommandStart>
	  <WatchAndForceTable>
		<Capacity>16</Capacity>
		<WatchAndForceTableEntry name=""Cmd"" />
		<WatchAndForceTableEntry name=""Execute"" />
		<WatchAndForceTableEntry name=""ExecState"" />
		<WatchAndForceTableEntry name=""ErrorCode"" />
		<WatchAndForceTableEntry name=""_State[1]"" />
		<WatchAndForceTableEntry name=""_StateName"" />
		<WatchAndForceTableEntry name=""I_SensorA"" />
		<WatchAndForceTableEntry name=""I_SensorB"" />
		<WatchAndForceTableEntry name=""_ValveA"" />
		<WatchAndForceTableEntry name=""_ValveB"" />
		<WatchAndForceTableEntry name=""_ValveOpenClamping"" />
	  </WatchAndForceTable>
	</Device>";

			var serializer = new ConfigurationContainer().InspectingType<SiemensDeviceTemplate>()
														 .EnableImplicitTyping(typeof(SiemensDeviceTemplate))
														 .Create()
														 .ForTesting();

			serializer.Deserialize<SiemensDeviceTemplate>(content)
					  .WatchAndForceTableEntires.Should()
					  .NotBeNull()
					  .And.Subject.Should()
					  .NotBeEmpty();
		}
	}
}