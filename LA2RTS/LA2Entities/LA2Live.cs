namespace LA2RTS.LA2Entities
{
    public class LA2Live : LA2Spawn
    {

        public int AbnormalID;// : Cardinal; айди получившийся из наборов флагов.Примеры ниже.
        //Abnormals : TBuffList; Для ГОД+ хроник.
        public string Ally;//: string; Имя альянса
        public int AllyID;//: Cardinal; ID альянса в который входит объект
        public bool Attackable;//: Boolean; Свободно атакуемый(без ctrl)
        public int AtkOID;//: Cardinal; OID объекта который атакует
        public int AtkTime;//: Cardinal; время когда начал атаковать
        //Buffs: TBuffList; Бафы объекта(доступны для нашего чара, пета и сопартийцев)
        //Cast: TL2Effect; Скил который объект кастует в данный момент.Актуально если Cast.EndTime > 0, иначе объект в данный момент не кастует.
        public string Clan;//: string; Имя клана
        public int ClanID;//: Cardinal; ID клана в который входит объект
        public int CurHP;//: Cardinal; Точное количество жизней
        public int CurMP;//: Cardinal; Точное количество маны
        public bool Dead;//: boolean; Жив или убит
        public bool Dropped;//: Boolean; Объект выронил предмет или нет(Dead должен быть True)
        public long Exp;//: Int64; Опыт
        public long EXP2;//: Int64;
        public int Fishing;//: Integer;
        public bool Fly;//: Boolean; This is Fly, a member of class TL2Live.
        public int HP;//: Cardinal; Текущее кол-во HP в процентах
        public bool InCombat;//: Boolean; Объект находится в комбате или нет
        public bool IsMember;//: Boolean; Является объект членом группы или нет
        public int Karma;//: Integer; Карма(начиная с GoD может быть как отрицательной (PK) так и положительной(репутация))
        public int Level;//: Byte; Уровень
        public int Load;//: Cardinal; Загруженность(проценты) (доступен для нашего чара или петов)
        public int MaxHP;//: Cardinal; Максимальное количество ХП
        public int MaxMP;//: Cardinal;
        public bool Moved;// : Boolean; Движется ли объект?
        public int MP;//: Cardinal; Текущее кол-во MP в процентах
        public int MyAtkTime;//: Cardinal; когда я его атаковал?
        public bool PK;//: Boolean; Player Killer
        public bool PvP;//: Boolean; Объект находится в режиме PvP
        public bool Running;//: Boolean; Объект движется пешком или бегом
        public bool Sitting;//: Boolean; Сидит?
        public int SP;//: Cardinal; Очки SP
        public double Speed;//: Double;
        public bool Sweepable;//: Boolean; Можно свипать?
        //Target: TL2Live; Цель объекта
        public int TargetOID;
        public int Team;//: Byte; для пвп серверов(красное синие подсвечивание), так же мобы "чемпионы"
        public int TeleportDist;//: Cardinal; Дистанция последней телепортации
        public int TeleportTime;//: Cardinal; Время последней телепортации
        public string Title;

        public int ToX;//: Integer; Координаты куда направился объект.
        public int ToY;//: Integer;
        public int ToZ;//: Integer;

        public LA2Live() :base()
        {

        }

        public LA2Live(int oid) : base(oid)
        {

        }

        internal void UpdateFull(LA2Live obj)
        {
            base.UpdateFull(obj);

            AbnormalID = obj.AbnormalID;
            Ally = obj.Ally;
            AllyID = obj.AllyID;
            AtkOID = obj.AtkOID;
            AtkTime = obj.AtkTime;
            Attackable = obj.Attackable;
            Clan = obj.Clan;
            ClanID = obj.ClanID;
            CurHP = obj.CurHP;
            CurMP = obj.CurMP;
            Dead = obj.Dead;
            Dropped = obj.Dropped;
            Exp = obj.Exp;
            EXP2 = obj.EXP2;
            Fishing = obj.Fishing;
            Fly = obj.Fly;
            HP = obj.HP;
            InCombat = obj.InCombat;
            IsMember = obj.IsMember;
            Karma = obj.Karma;
            Level = obj.Level;
            Load = obj.Load;
            MaxHP = obj.MaxHP;
            MaxMP = obj.MaxMP;
            Moved = obj.Moved;
            MP = obj.MP;
            MyAtkTime = obj.MyAtkTime;
            PK = obj.PK;
            PvP = obj.PvP;
            Running = obj.Running;
            Sitting = obj.Sitting;
            SP = obj.SP;
            Speed = obj.Speed;
            Sweepable = obj.Sweepable;
            TargetOID = obj.TargetOID;
            Team = obj.Team;
            TeleportDist = obj.TeleportDist;
            TeleportTime = obj.TeleportTime;
            Title = obj.Title;

            ToX = obj.ToX;
            ToY = obj.ToY;
            ToZ = obj.ToZ;
        }

        internal void UpdateQuick(LA2Live obj)
        {
            base.UpdateQuick(obj);

            AbnormalID = obj.AbnormalID;

            Attackable = obj.Attackable;
            Dead = obj.Dead;
            Dropped = obj.Dropped;
            Sweepable = obj.Sweepable;

            HP = obj.HP;
            MP = obj.MP;
            CurHP = obj.CurHP;
            CurMP = obj.CurMP;
            MaxHP = obj.MaxHP;
            MaxMP = obj.MaxMP;

            AtkTime = obj.AtkTime;
            AtkOID = obj.AtkOID;
            Moved = obj.Moved;
            Running = obj.Running;
            Sitting = obj.Sitting;
            TargetOID = obj.TargetOID;
            InCombat = obj.InCombat;

            PK = obj.PK;
            PvP = obj.PvP;

            Speed = obj.Speed;
            ToX = obj.ToX;
            ToY = obj.ToY;
            ToZ = obj.ToZ;
        }
    }
}
