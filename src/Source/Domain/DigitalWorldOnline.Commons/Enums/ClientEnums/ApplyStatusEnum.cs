namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum ApplyStatusEnum : int
    {
		APPLY_HP = 1,               //HP
		APPLY_DS,                   //DS
		APPLY_MAXHP,                // Extensão Max HP.
		APPLY_MAXDS,                // Extensão máxima DS.
		APPLY_AP,                   // Aumentar o poder de ataque
		APPLY_CA,                   // Aumento de dano crítico
		APPLY_DP,                   // Aumento da defesa
		APPLY_EV,                   // Aumento de evasão
		APPLY_MS,                   // Aumento da velocidade do movimento
		APPLY_AS,                   // Aumentar a velocidade de ataque
		APPLY_AR,                   // Aumentar a interseção de ataque
		APPLY_HT,                   // Aumento de sucesso
		APPLY_FP,                   // Aumento do nível de fadiga
		APPLY_FS,                   // Aumentar a intimidade
		APPLY_EXP,                  // Experiência
		APPLY_POWERAPPLYRATE,       // Melhorar a capacidade de aplicação de soquete

		APPLY_BL = 17,  // Taxa de bloco
		APPLY_DA,       // Danos gerais de ataque
		APPLY_ER,       // Probabilidade de evasão de ataque geral
		APPLY_AllParam, // O parâmetro inteiro subindo (esse valor é usado como um parameter)
		APPLY_SER,      // Probabilidade de evasão de habilidades
		APPLY_SDR,      // Probabilidade de defesa de habilidades
		APPLY_SRR,      // Habilidade humilde
		APPLY_SCD,      // Dano de habilidade.
		APPLY_SCR,      // Habilidade probabilidade criativa
		APPLY_HRR,      // Recuperação da HP.
		APPLY_DRR,      // DS Recovery.
		APPLY_MDA,      // Redução normal de dano
		APPLY_HR,       // Ataque geral atingiu a probabilidade
		APPLY_DSN,      // Aumento de recuperação da natureza DS
		APPLY_HPN,      // Aumento da recuperação da natureza da HP
		APPLY_STA,      // Visão geral
		APPLY_UB,       //invencibilidade
		APPLY_ATTRIBUTTE,   // Boost Professional.
		APPLY_CC,           // code coroa.
		APPLY_CR,           // transversal carregador
		APPLY_DOT,          // dano sustentado
		APPLY_DOT2,         // dano atrasado
		APPLY_STUN,         // Nenhum controle
		APPLY_DR,           // Damage Reflection.
		APPLY_AB,           // absorção de danos // 41

		//#ifdef KSJ_ADD_MEMORY_SKILL_20140805
		APPLY_HPDMG,        // Aumento dos danos, dependendo do restante quantidade HP, 42 vezes
		APPLY_ATDMG,        // Aumentar danos de acordo com as propriedades
		APPLY_HPDEF,        // Dano reduzido de acordo com o restante HP
		APPLY_ATDEF,        // Diminuir o dano dependendo das propriedades
		APPLY_PROVOKE,      // Provocação
		APPLY_INSURANCE,    // Recuperação de florescência

		// RENEWAL_TAMER_SKILL_20150923.
		APPLY_CAT,          // Dano crítico
		APPLY_RDD
	}
}
