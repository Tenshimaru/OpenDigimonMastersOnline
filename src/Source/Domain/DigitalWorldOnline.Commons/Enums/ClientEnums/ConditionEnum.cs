namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
	public enum ConditionEnum
	{
		Default = 0,
		Ride = 1,
		Away = 2,

		/// <summary>
		/// Sets the tamer as a tamer shop.
		/// </summary>
		TamerShop = 4,

		/// <summary>
		/// Used on the preparing shop stage (tamer/consigned).
		/// </summary>
		PreparingShop = 8,

		PCBang = 0x00000010,    // 오라 체크 pc방 컨디션
		Peace = 0x00000020, // 평화선언상태
		s7 = 0x00000040,    // 
		s8 = 0x00000080,    // 

		s9 = 0x00000100,    // 
		Scanner0 = 0x00000200,  // 1:스캐너 장착, 0:스캐너 미장착
		Scanner1 = 0x00000400,  // 1:자신의 소유로 등록된 스캐너, 0:자신의 소유로 등록되지 아니한 스캐너
		Scanner2 = 0x00000800,  // 1:한정판 스캐너, 0:일반판 스캐너

		s13 = 0x00001000,   // 
		s14 = 0x00002000,   // 
		s15 = 0x00004000,   // 
		s16 = 0x00008000,

		s17 = 0x00010000,   // 
		S18 = 0x00020000,   // 
		Guild = 0x00040000, // Join a guild
		S20 = 0x00080000,   // 경험치-레벨 랭커

		S21 = 0x00100000,   // 크기 랭커
		Invisible = 0x00200000, // 투명인간
		
		/// <summary>
		/// Fatigue level are bigger than the current health.
		/// </summary>
		Fatigued = 0x00400000,

		Immortal = 0x00800000,    // 무적

		Run = 0x10000000,   // 뛰기
		Rest = 0x20000000,  // 휴식중
		Die = 0x40000000,   // 죽음
		//Battle = 0x80000000,    // 전투중

		Return = 0x02000000,    // 몬스터가 되돌아가는 상태, 무적과는 좀 다르게 표현
	};
}