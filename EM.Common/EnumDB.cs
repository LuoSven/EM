using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EMTop.Common
{
    public class EnumDB
    {
        public static string GetGender(object p_Gender)
        {
            return ((Gender)Convert.ToInt32(p_Gender)).ToString();
        }

        public static string Personality(object p_Gender)
        {
            return ((Personality)Convert.ToInt32(p_Gender)).ToString();
        }
    }

    public enum Gender
    {
        男 = 1,
        女 = 0
    }

    public enum Personality
    {
        外向为主 = 1,
        内向为主 = 0
    }

    /// <summary>
    /// TB_Stu_Resume_Status
    /// </summary>
    public enum Privacy
    {
        完全公开 = 1,
        部分公开 = 11,//（向我申请的、我关注的企业公开）
        对申请企业公开 = 31,
    }

    /// <summary>
    /// 学生状态
    /// </summary>
    public enum StudentStatus
    {
        [Description("注册")]
        Registered = 0,
        [Description("正常使用")]
        Active = 1,
        [Description("黑名单")]
        Blacklist = 99
    }

    /// <summary>
    /// 企业状态 TB_Enterprise
    /// </summary>
    public enum EtpStatus
    {
        [Description("新注册待审核")]
        Registered = 1, //已停用，自然注册即设置为EtpStatus.Normal
        [Description("正常运行")]
        Normal = 11,
        [Description("服务下载")]
        Downloaded = 21,
        [Description("隐藏公司")]
        Shadow = 31,
        [Description("无效")]
        Invalid = 99
    }

    /// <summary>
    /// 企业级别 TB_Enterprise
    /// </summary>
    public enum CustomerLevel
    {
        [Description("普通")]
        Regular = 1,
        [Description("VIP")]
        VIP = 2,
        [Description("内部测试")]
        InternalTest = 99
    }

    /// <summary>
    /// 企业账号状态 TB_Enterprise_Account
    /// </summary>
    public enum EtpAccountStatus
    {
        [Description("待处理")]
        Undetermined = 0,
        [Description("已通过")]
        Approved = 1,
        [Description("已拒绝")]
        Refuse = 2,
        [Description("已停用")]
        Cancellation = 3
    }

    /// <summary>
    /// 职位状态
    /// </summary>
    public enum PositionStatus
    {
        [Description("发布中")]
        Publish = 1,
        [Description("待迁移")]
        ToMigrate = 21,
        [Description("发布结束")]
        Expired = 31,
        [Description("暂停")]
        Paused = 61,
        [Description("强制关闭")]
        ForceClose = 91,
        [Description("已删除")]
        Deleted = 99,
        [Description("待发布")]
        WaitingApprove = 41
    }

    /// <summary>
    /// 职位类型 TB_Position_Element
    /// </summary>
    public enum PositionType
    {
        [Description("全职")]
        FullTime = 1,
        [Description("实习")]
        PartTime = 2,
        [Description("实习")]
        Internship = 3
    }

    public enum POrder
    {
        [Description("发布时间")]
        DeployTime = 1,
        [Description("申请")]
        Apply = 2,
        [Description("关注")]
        Focus = 3,
        [Description("收藏")]
        Collect = 3,
    }

    /// <summary>
    /// 简历类型
    /// </summary>
    public enum LanguageVersion
    {
        [Description("中文")]
        Chinese = 0,
        [Description("英文")]
        English = 1,
    }

    public enum SessionKey
    {
        [Description("默认")]
        Default,
        [Description("企业登录")]
        EnterpriseLogin,
        [Description("企业账号")]
        EnterpriseAccount,
        [Description("企业找回密码")]
        EnterpriseFindPwd,
        [Description("学生登录")]
        StudentLogin,
        [Description("学生账号")]
        StudentAccount,
        [Description("学生找回密码")]
        StudentFindPwd
    }
    /// <summary>
    /// 职位申请状态  TB_S_Position
    /// </summary>
    public enum ApplyStatus
    {
        [Description("未处理")]
        Apply = 21,
        [Description("已读")]
        Read = 25,
        [Description("感兴趣")]
        Interested = 28,
        [Description("已通知面试")]
        Approved = 31,
        [Description("不合适")]
        Refused = 61,
        [Description("已删除")]
        Deleted = 99,
        [Description("曾经感兴趣过")]
        BeenInterested = 71,

        /// <summary>
        /// 仅用于 收藏操作，数据库不存在对应的状态（星标的简历用’收藏日期‘表示）
        /// </summary>
        [Description("星标操作")]
        FavorJustForAction = 999,
        /// <summary>
        /// 仅用于 取消收藏操作，数据库不存在对应的状态（星标的简历用’收藏日期‘表示）
        /// </summary>
        [Description("取消星标操作")]
        UnFavorJustForAction,
        /// <summary>
        /// 仅用于 标记到回收箱，数据库不存在对应的状态（删除的简历用’删除日期‘表示）
        /// </summary>
        [Description("标记到回收箱")]
        ToRecycleBinJustForAction,
        /// <summary>
        /// 仅用于 从回收箱回收，数据库不存在对应的状态（回收箱的简历用’删除日期‘表示）
        /// </summary>
        [Description("从回收箱回收")]
        BackFromRecycleBinJustForAction,

    }

    /// <summary>
    /// 申请状态组
    /// </summary>
    public enum ApplyStatusGroup
    {
        [Description("状态")]
        Default = 0,
        [Description("未处理")]
        NoHandle = 2,
        [Description("已通过")]
        Approved = 3,
        [Description("已拒绝")]
        Refuse = 9
    }

    public enum FollowStatus
    {
        [Description("未关注")]
        None = 0,
        [Description("学生关注企业")]
        EtpFollowed = 1,
        [Description("企业关注学生")]
        StuFollowed = 2,
        [Description("互相关注")]
        BothFollowed = 3
    }

    /// <summary>
    /// 暂未使用
    /// </summary>
    public enum ChinaArea
    {
        [Description("东北地区")]
        Northeast = 1,
        [Description("华北地区")]
        NorthChina = 2,
        [Description("华东地区")]
        EastChina = 3,
        [Description("华中地区")]
        CentralChina = 4,
        [Description("华南地区")]
        SouthChina = 5,
        [Description("西南地区")]
        Southwest = 6,
        [Description("西北地区")]
        Northwest = 7,
        [Description("港澳台地区")]
        HMT = 8
    }

    public enum ErrorCode
    {
        [Description("账户锁定")]
        AccountLockedOut = 0,
        [Description("账户没有审核")]
        AccountNotApproved = 1,
        [Description("账户正常")]
        AccountNormal = 2,
        [Description("账户异常")]
        AccountInvalid = 3,
        [Description("账户不存在")]
        AccountNotExists = 4,
        [Description("账户已存在")]
        AccountExists = 5,
        [Description("账户和密码不匹配")]
        AccountAndPasswordNotMatch = 6,
        [Description("账户进入黑名单")]
        AccountBlackList = 99,
        [Description("用户名不存在")]
        UserNameNotExists = 100,
        [Description("用户名和邮箱不匹配")]
        UserNameEmailNotMatch = 101,
        [Description("链接失效")]
        LinkExpired = 200,
    }

    public enum NoticficationType
    {
        [Description("留言")]
        Message = 0,
        [Description("操作")]
        Action = 1,
    }



    public enum AdminUserType
    {
        [Description("管理员")]
        Admin = 0,
        [Description("业务专员")]
        BD = 1,
        [Description("顾问")]
        Consultant = 2,
        [Description("普通职员")]
        Clerk = 3,
        [Description("超级管理员")]
        SuperAdmin = 99
    }

    /// <summary>
    /// 企业处理状态 TB_Enterprise
    /// </summary>
    public enum EtpProcessStatus
    {
        [Description("未处理")]
        Initial = 0,
        [Description("正在处理")]
        Process = 1,
        [Description("正在跟进")]
        Followed = 2,
        [Description("账号待审核")]
        AccountGenerated = 3,
        [Description("审核已通过")]
        AccountApproved = 4,
        [Description("审核未通过")]
        AccountRejected = 5,
        [Description("已忽略")]
        CancelFollowed = 6
    }

    public enum ModuleType
    {
        [Description("企业基本信息")]
        CustomerBasicInfo = 0,
        [Description("校招信息")]
        RecuritementInfo = 1
    }

    public enum JobFairType
    {
        [Description("毕业生招聘会")]
        Graduate = 0,
        [Description("综合招聘会")]
        General = 1
    }

    /// <summary>
    /// 求职指导中文章类型 TB_Article
    /// </summary>
    public enum ArticleType
    {
        [Description("求职指导")]
        JobGuidance = 1,

        [Description("校招攻略")]
        HRShare = 2,

        [Description("猎头说")]
        Expirence = 3,

        [Description("健康贴士")]
        HealthTip = 4,

        [Description("星座运势")]
        Horoscope = 5,

        [Description("每日黄历")]
        DailyAlmanac = 6,

        [Description("开口长笑")]
        Laugh = 7,
        [Description("HR学堂")]
        HRClass = 8
    }

    #region 邮件相关
    public enum EmailType
    {
        [Description("未知")]
        Unknown = 0,
        [Description("转发简历")]
        ForwardResume = 1,

        [Description("学生注册欢迎")]
        RegisterWelcome = 100,
        [Description("学生忘记密码")]
        StuFindPassword,
        [Description("宣讲会订阅")]
        JobSeminarFeed,
        [Description("简历点评")]
        ResumeAdvice,
        [Description("职位点评")]
        ApplyAdvice,
        [Description("面试点评")]
        InterviewAdvice,
        [Description("职位推送")]
        PushPosition,
        [Description("职业理想职位推送")]
        PushDreamPosition,
        //
        [Description("求职指导推广")]
        JobGuideAD,
        [Description("学生拜年")]
        NewYearStu,
        [Description("投递职位收到面试通知")]
        ReceiveInterviewNotification,
        [Description("职位邀请")]
        InvitePosition,
        [Description("导师回复")]
        TutorFeedback,
        [Description("对话HR")]
        CommunicateHR,
        [Description("公司新动态")]
        CompanyUpdate,
        [Description("简历通过筛选")]
        ResumePassed,
        [Description("学生忘记密码6位短码")]
        StuFindPasswordShortCode,

        [Description("学生转发简历")]
        StuForwardResume ,

        [Description("企业拜年")]
        NewYearEtp = 200,
        [Description("企业注册")]
        EnterpriseRegistration,
        [Description("企业激活")]
        CheckPass,
        //
        [Description("企业简历激活")]
        EtpResumeActivation,
        [Description("企业找回密码")]
        ComFindPassword,
        [Description("激活企业收到新简历")]
        RegistNewResume,
        [Description("未激活企业收到新简历")]
        UnRegistNewResume,
        [Description("审核未通过邮件")]
        CheckFailed,
        [Description("收到简历推荐")]
        NewResume,
        [Description("对话Hr收到新提问")]
        NewQA,
        [Description("激活邮件由系统一键发送")]
        NewNewEtpActivation,
        [Description("企业认证会员申请已通过审核")]
        PassCertificate,
        [Description("企业认证会员申请未能通过审核")]
        FailCertificate,
    }

    public enum EmailDeliveryStatus
    {
        [Description("等待发送")]
        Wait = 0,
        [Description("发送中")]
        Delivery = 1,
        [Description("发送成功")]
        Success = 2,
        [Description("发送失败")]
        Fail = 3,
        [Description("取消发送")]
        Cancel = 4
    }

    public enum EtpActivationStatus
    {
        [Description("等待发送激活邮件")]
        Wait = 0,
        [Description("等待激活中")]//邮件已发送，激活过程进行中
        InProcess = 1,
        [Description("激活成功")]
        Success = 2,
        [Description("激活失败")]
        Failed = 3
    }

    #endregion


    #region 简历相关
    public enum ClassRanking
    {
        [Description("前10%")]
        Percent10 = 1,
        [Description("前30%")]
        Percent30 = 2,
        [Description("前50%")]
        Percent50 = 3,
        [Description("其它")]
        Other = 4,
    }


    public enum Degree
    {
        [Description("不限")]
        Default = 0,
        [Description("大专")]
        College = 1,
        [Description("本科")]
        Bachelor = 2,
        [Description("硕士")]
        Master = 3,
        [Description("博士")]
        Doctor = 4
    }

    public enum Grade
    {
        [Description("大一")]
        Fresher = 1,
        [Description("大二")]
        Sophomore = 2,
        [Description("大三")]
        Junior = 3,
        [Description("大四")]
        Senior = 4,
        [Description("大五")]
        Five = 5,
        [Description("大六")]
        Six = 6,
        [Description("研一")]
        MasterFirst = 7,
        [Description("研二")]
        MasterSecond = 8,
        [Description("研三")]
        MasterThird = 9,
        [Description("博一")]
        DoctorFirst = 10,
        [Description("博二")]
        DoctorSecond = 11,
        [Description("博三")]
        DoctorThird = 12,
        [Description("博四")]
        DoctorFouth = 13,
    }

    public enum LanguageLevel
    {
        [Description("精通")]
        Proficient = 1,
        [Description("熟练")]
        Skilled = 2,
        [Description("良好")]
        Good = 3,
        [Description("一般")]
        General = 4,
    }

    public enum LanguageType
    {
        [Description("英语")]
        English = 1,
        [Description("日语")]
        Janpanese ,
        [Description("俄语")]
        Russian,
        [Description("法语")]
        French,
        [Description("德语")]
        Germany ,
        [Description("西班牙语")]
        Spanish,
        [Description("葡萄牙语")]
        Portuguese ,
        [Description("意大利语")]
        Italian ,
        [Description("韩语/朝鲜语")]
        Korean ,
        [Description("阿拉伯语")]
        Arabic ,
        [Description("粤语")]
        Cantonese ,
        [Description("沪语")]
        Shanghaiese ,
        [Description("闽南语")]
        Hokkien ,
    }

    public enum Politics
    {
        [Description("中共党员（含预备党员）")]
        Communist = 1,
        [Description("团员")]
        League = 2,
        [Description("民主党派")]
        Democratic = 3,
        [Description("群众")]
        Masses = 4
    }

    public enum EducationSystem
    {
        [Description("一年")]
        One = 1,
        [Description("二年")]
        Two = 2,
        [Description("三年")]
        Three = 3,
        [Description("四年")]
        Four = 4,
        [Description("五年")]
        Five = 5,
        [Description("六年")]
        Six = 6,
        [Description("七年")]
        Seven = 7,
        [Description("八年")]
        Eight = 8,
    }


    public enum AttachType
    {
        [Description("简历")]
        Resume = 1,
        [Description("作品")]
        Production = 2,
        [Description("证书")]
        Certificate = 3
    }

    public enum Scholarship
    {
        [Description("1次以上")]
        One = 1,
        [Description("2次以上")]
        Two = 2,
        [Description("3次以上")]
        Three = 3,
        [Description("4次以上")]
        Four = 4,
        [Description("5次以上")]
        Five = 5
    }

    //public enum SchoolLevel
    //{
    //    [Description("985")]
    //    L985 = 4,
    //    [Description("211及以上")]
    //    L211 = 3,
    //    [Description("一本及以上")]
    //    YiBen = 2,
    //    [Description("本科及以上")]
    //    BenKe = 1,
    //    [Description("不限")]
    //    Default = 9,
    //}


    public enum SchoolLevel
    {
        [Description("985院校")]
        L985 = 1,
        [Description("211院校")]
        L211 = 2,
        [Description("普通一本院校")]
        Ben1 = 3,
        [Description("普通二本院校")]
        Ben2 = 4,
        [Description("普通三本院校")]
        Ben3 = 5,
        [Description("高职高专")]
        Zhuanke = 6,
        [Description("其他院校")]
        Others = 7
    }

    public enum ResumeFileStatus
    {
        [Description("不存在")]
        NotExist,
        [Description("存在")]
        Exist,
        [Description("等待生成或生成中")]
        Creating
    }

    #endregion




    #region 第三方api（微信）
    //消息类型 b8ac522c780e9be131bba6ef0bbba1ec
    public enum WeChatMessageType
    {
        [Description("未知")]
        Unknown = 0,
        [Description("转发简历")]
        ForwardResume = 1,
        [Description("学生注册欢迎")]
        RegisterWelcome = 100,
        [Description("学生忘记密码")]
        StuFindPassword,
        [Description("宣讲会订阅")]
        JobSeminarFeed,
        [Description("简历点评")]
        ResumeAdvice,
        [Description("职位点评")]
        ApplyAdvice,
        [Description("面试点评")]
        InterviewAdvice,
        [Description("职位推送")]
        PushPosition,
        [Description("职业理想职位推送")]
        PushDreamPosition,
        //
        [Description("求职指导推广")]
        JobGuideAD,
        [Description("学生拜年")]
        NewYearStu,
        [Description("投递职位收到面试通知")]
        ReceiveInterviewNotification,
        [Description("职位邀请")]
        InvitePosition,
        [Description("导师回复")]
        TutorFeedback,
        [Description("对话HR")]
        CommunicateHR,
        [Description("公司新动态")]
        CompanyUpdate,
        [Description("简历通过筛选")]
        ResumePassed,
        [Description("简历被拒绝")]
        ResumeRefused,
        [Description("学生忘记密码6位短码")]
        StuFindPasswordShortCode,


        [Description("企业拜年")]
        NewYearEtp = 200,
        [Description("企业注册")]
        EnterpriseRegistration,
        [Description("企业激活")]
        CheckPass,
        //
        [Description("企业简历激活")]
        EtpResumeActivation,
        [Description("企业找回密码")]
        ComFindPassword,
        [Description("激活企业收到新简历")]
        RegistNewResume,
        [Description("未激活企业收到新简历")]
        UnRegistNewResume,
        [Description("审核未通过邮件")]
        CheckFailed,
        [Description("收到简历推荐")]
        NewResume,
        [Description("对话Hr收到新提问")]
        NewQA,
        [Description("激活邮件由系统一键发送")]
        NewNewEtpActivation,
        [Description("企业认证会员申请已通过审核")]
        PassCertificate,
        [Description("企业认证会员申请未能通过审核")]
        FailCertificate,
    
    }
    public enum WeChatPlatformType
    {
        Default,
        Subscription,
        Service
    }

    public enum QRCodeType
    {
        QR_SCENE,
        QR_LIMIT_SCENE
    }
    public enum WeChatApiType
    {
        [Description("获取接口调用凭证")]
        GetAccessToken = 0,
        [Description("客服接口-发消息")]
        MessageCustom,
        [Description("发送模板消息")]
        MessageTemplate,
        [Description("创建二维码ticket")]
        GetQRCode
    }
    public enum RequestMethodType
    {
        [Description("Get")]
        Get = 0,
        [Description("Post")]
        Post
    }

    #endregion


    public enum PublishDate
    {
        [Description("近1天")]
        OneDay = 1,
        [Description("近2天")]
        TwoDay = 2,
        [Description("近3天")]
        ThreeDay = 3,
        [Description("近1周")]
        OneWeek = 4,
        [Description("近2周")]
        TwoWeek = 5,
        [Description("近1月")]
        OneMonth = 6,
        [Description("近2月")]
        TwoMonth = 7
    }

    public enum SalaryRange
    {
        [Description("1K以上")]
        Above1k = 1,
        [Description("1.5K以上")]
        Above1dot5k = 2,
        [Description("2K以上")]
        Above2k = 3,
        [Description("2.5K以上")]
        Above2dot5k = 4,
        [Description("3K以上")]
        Above3k = 5,
        [Description("4K以上")]
        Above4k = 6,
        [Description("5K以上")]
        Above5k = 7,
        [Description("6K以上")]
        Above6k = 8,
        [Description("8K以上")]
        Above8k = 9,
        [Description("10K以上")]
        Above10k = 10
    }

    public enum Constellation
    {
        [Description("白羊")]
        白羊座 = 1,
        [Description("金牛")]
        金牛座 = 2,
        [Description("双子")]
        双子座 = 3,
        [Description("巨蟹")]
        巨蟹座 = 4,
        [Description("狮子")]
        狮子座 = 5,
        [Description("处女")]
        处女座 = 6,
        [Description("天秤")]
        天秤座 = 7,
        [Description("天蝎")]
        天蝎座 = 8,
        [Description("射手")]
        射手座 = 9,
        [Description("魔羯")]
        魔羯座 = 10,
        [Description("水瓶")]
        水瓶座 = 11,
        [Description("双鱼")]
        双鱼座 = 12
    }


    public enum BloodType
    {
        [Description("A型")]
        A = 1,
        [Description("B型")]
        B = 2,
        [Description("AB型")]
        AB = 3,
        [Description("O型")]
        O = 4,
        [Description("稀有血型")]
        X = 5
    }

    public enum MessageType
    {
        //企业自动回复
        [Description("自动回复")]
        AutoReply = 1,
        //以下1开头代表学生收到
        [Description("已读")]
        Read = 11,
        [Description("感兴趣,职位邀请")]
        Interest = 12,
        [Description("通知面试")]
        Interview = 13,
        [Description("未符合要求")]
        Refused = 14,
        [Description("系统消息")]
        StuSystemMsg = 19,

        //以下2开头代表企业收到
        [Description("未读个数")]
        UnReadCount = 21,
        [Description("回复学生QA")]
        CompanyAnswer = 22,
        [Description("公司新动态")]
        CompanyUpdate = 23,
        [Description("系统消息")]
        CorpSystemMsg = 29,

        //以下3开头代表院系收到
        [Description("学生申请")]
        StuApply = 31,
        [Description("企业申请")]
        CorpApply = 35,
        [Description("系统消息")]
        CollegeSystemMsg = 39,
        //回复提问
        [Description("导师回复提问")]
        TutorAnswer = 50
    }

    public enum RoleType
    {
        [Description("学生")]
        Student = 1,
        [Description("企业")]
        Company = 2,
        [Description("院系")]
        College = 3,
        [Description("导师")]
        Advisor = 4,
        [Description("系统")]
        System = 5,
    }

    public enum CollegeQualifyType
    {
        [Description("学生")]
        Student = 1,
        [Description("企业")]
        Company = 2
    }

    public enum CollegeQualifyStatus
    {
        [Description("未处理")]
        Wait = 0,
        [Description("已通过")]
        Accept = 1,
        [Description("已拒绝")]
        Reject = 2,
        [Description("无记录")]
        None = 3,
        [Description("已失效")]
        Invalid = 4
    }


    public enum ActionActorRole
    {
        [Description("学生")]
        Student = 1,
        [Description("企业")]
        Corporation = 2,
        [Description("学院")]
        College = 3
    }

    public enum ActionTargetType
    {
        [Description("职位")]
        Position = 1,
        [Description("问题")]
        Question = 2,
        [Description("新闻")]
        News = 3,
        [Description("企业")]
        Corporation = 4,
        [Description("学生")]
        Student = 5,
        [Description("基本信息")]
        Intro = 6,
        [Description("教育经历")]
        Education = 7,
        [Description("实习经历")]
        Internship = 8,
        [Description("基本信息")]
        BasicInfo = 9
    }


    public enum EtpActivityType
    {
        [Description("发布职位")]
        Position = 1,
        [Description("发布司生活")]
        Life = 2,
        [Description("上传照片")]
        Photo,
        [Description("视频")]
        Video
    }

    public enum MaterialType
    {
        [Description("营业执照")]
        License = 1,
        [Description("组织机构代码证")]
        OrganCodeCerti = 2
    }

    public enum StatisticsType
    {
        [Description("企业")]
        Corporation = 1,
        [Description("职位")]
        Position = 2,
        [Description("招聘会")]
        JobFair = 3,
        [Description("求职指导")]
        JobGuidance = 4,
        [Description("员工故事")]
        StaffStory = 5,
        [Description("宣讲会")]
        JobSeminar = 6,
    }

    public enum PushType
    {
        [Description("系统推荐")]
        System = 1,
        [Description("普通推荐")]
        Normal = 2,
        [Description("特别推荐")]
        Special = 3
    }

    public enum DisplayType
    {
        [Description("显示")]
        Show = 1,
        [Description("隐藏")]
        Hidden = 0
    }


    public enum Interval
    {
        [Description("天")]
        OneDay = 1,
        [Description("3天")]
        ThreeDays = 3,
        [Description("7天")]
        SevenDays = 7

    }

    public enum AdviceType
    {
        [Description("系统点评")]
        System = 0,
        [Description("简历点评")]
        Resume = 1,
        [Description("职位点评")]
        Apply = 2,
        [Description("面试点评")]
        Interview = 3

    }

    public enum PushNewsType
    {
        [Description("求职意向")]
        Intention = 0,
        [Description("学院推荐")]
        College = 1,
        [Description("企业关注")]
        Follow = 2,
        [Description("同专业")]
        Major = 3,
        [Description("论坛热帖")]
        BBS = 4,
        [Description("文章推荐")]
        Article = 5,
        [Description("导师指导")]
        Advice = 6,
        [Description("职位邀请")]
        Invite = 7

    }
    public enum VoteType
    {
        [Description("默认")]
        None = 0,
        [Description("有用")]
        Useful = 1,
        [Description("无用")]
        Useless = 2
    }




    public enum OnlineAlbumType
    {
        [Description("公司视频")]
        Company = 1,
        [Description("司生活")]
        Life = 2
    }

    public enum ConfigType
    {
        [Description("名企墙")]
        FamousWall = 1,
        [Description("热门文章标签")]
        HotTag = 2,
        [Description("热门文章")]
        HotArticle = 3,
        [Description("重点推荐文章")]
        RecommendedArticle = 4,
        [Description("热门企业文章标签")]
        HotHrTag = 5,
        [Description("热门企业文章")]
        HotHrArticle = 6,
        [Description("重点企业推荐文章")]
        RecommendedHrArticle = 7,
        [Description("移动端首页推荐职位")]
        MobileHomeJobs = 8,
        [Description("企业中心文章图片")]
        CropHomePic = 9,
        [Description("企业中心校招攻略")]
        CropHomeArticle = 10,
        [Description("侧栏推荐企业")]
        HotEtps = 11,
        [Description("导航栏热搜职位")]
        DaoHangLanHotSearchPosition = 12,
        [Description("搜索栏热搜职位")]
        SouSuoLanHotSearchPosition = 13
    }



    public enum InternLength
    {
        //3个月以下、3-6个月、6个月以上
        [Description("3个月以下")]
        InThreeMonth = 1,
        [Description("3-6个月")]
        ThreeToSixMonth = 2,
        [Description("6个月以上")]
        OverSixMonth = 3,
    }

    public enum InternDays
    {
        //2天、3天、4天、5天
        [Description("2天")]
        TwoDays = 2,
        [Description("3天")]
        ThreeDays = 3,
        [Description("4天")]
        FourDays = 4,
        [Description("5天")]
        FiveDays = 5,
    }

    public enum InternSalaryType
    {
        //2天、3天、4天、5天
        [Description("月薪")]
        Month = 1,
        [Description("日薪")]
        Day = 2,
        [Description("时薪")]
        Hour = 3,

    }


    public enum AlbumSourceType
    {
        [Description("普通")]
        Normal = 1,
        [Description("微信HR上传")]
        WeChatHrUpload = 2
    }

    public enum ObjectiveType
    {
        [Description("全职")]
        FullTime = 1,
        [Description("实习")]
        Internship = 2,
        [Description("全职+实习")]
        All = 3,
    }

    public enum WeChatUploadStatus
    {
        [Description("待上传")]
        Wait = 1,
        [Description("上传中")]
        Uploading = 2,
        [Description("上传成功")]
        Success = 3,
        [Description("上传失败")]
        Fail = 4
    }


    public enum WeChatMediaType
    {
        [Description("文本")]
        text = 1,
        [Description("图片")]
        image = 2,
        [Description("语音")]
        voice = 3,
        [Description("视频")]
        video = 4
    }

    public enum PushStuMark
    {
        [Description("未处理")]
        None = 0,
        [Description("感兴趣")]
        Interested = 1,
        [Description("忽略")]
        Ignore = 2
    }

    public enum PushEtpMark
    {
        [Description("未处理")]
        None = 0,
        [Description("已邀请投递")]
        Invited = 1,
        [Description("已标记为不合适")]
        Ignore = 2,
        [Description("已邀请投递其他职位")]
        InvitedAnother = 3
    }

    public enum CityValueType
    {
        CityId = 0,
        CityName = 1,
        CityPY = 2,
    }

    public enum EtpAppreciationType 
    {
        [Description("校园直通车")]
        ZTC = 1,
        [Description("校园猎头")]
        Hunter =2
    }

    public enum AppreciationApplyProcessStatus
    {
        [Description("未处理")]
        Initial = 1,
        [Description("已认领")]
        Claimed = 2,
        [Description("跟进中")]
        Processing = 3,
        [Description("已处理")]
        ProcessDone = 4,
    }

    /// <summary>
    /// 学生账号注册类型
    /// </summary>
    public enum StuAccountRegType
    {
        [Description("未知")]
        Default = 0,
        [Description("移动端")]
        Touch = 1
    }

    /// <summary>
    /// 企业账号注册类型
    /// </summary>
    public enum EtpAccountRegType
    {
        [Description("自然流量注册")]
        BySelfReg = 1,
        [Description("SEM流量注册")]
        BySEMSelfReg,
        [Description("人工激活链接注册")] // /account/activation?etpid=123&code=12321321323
        ByActivationLink,
        [Description("EDM激活链接注册")] // /account/activation?etpid=123&code=12321321323&adid=edm
        ByEDMActivationLink,
        [Description("EDM注册链接")] // /account/login?adid=edm
        ByEDMRegisterLink,
        [Description("其他")]
        Else = 9
    }

    public enum SubDataContentCataType
    {
        [Description("大礼包_最新更新")]
        DaLiBao_Recently = 1,
        [Description("大礼包_下载榜单")]
        DaLiBao_DownList,
        [Description("大礼包_考研精品资料_文件列表")]
        DaLiBao_KaoYanPian_DownList,
        [Description("大礼包_简历宝典_文件列表")]
        DaLiBao_QiuZhiPian_DownList,
        [Description("大礼包_面试秘籍_下载列表")]
        DaLiBao_MianShiMiJi_DownList,
        [Description("大礼包_最佳求职网站_排行列表")]
        DaLiBao_ZuiJiaQiuZhi_RankList,
        [Description("大礼包_求职网站_好评列表")]
        DaLiBao_QiuZhiWebSite_HighPriseList,
        [Description("大礼包_职业锚_推荐下载列表")]
        DaLiBao_ZhiYeMao_TuiJianDownloadList,
        [Description("大礼包_报告下载_列表")]
        DaLiBao_BaoGaoXiaZai_List,

        [Description("打包下载考研精品资料下载链接_JustOne")]
        DaBaoXiaZai_KaoYanJingPin_FOrDefault,
        [Description("打包下载简历宝典_JustOne")]
        DaBaoXiaZai_JianLiBaoDian_FOrDefault,
        [Description("打包下载面试秘籍_JustOne")]
        DaBaoXiaZai_MianShiMiJi_FOrDefault

    }

    public enum OperationResult
    {
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Fail = 2,
        [Description("重复操作")]
        RepeatOperation = 3
    }

    public enum EnterpriseTTPLink 
    {
        [Description("新浪微博")]
        Sina,
        [Description("腾讯微博")]
        Tencent,
        [Description("人人网主页")]
        RenRen,
        [Description("微信公众号")]
        WeChat
    }

    /// <summary>
    /// 学生简历分数比例
    /// </summary>
    public enum StuProfileRatioType
    {
        [Description("30")]
        TB_S_Basic, //基本信息
        [Description("10")]
        TB_Stu_Evaluation, //自我简介
        [Description("30")]
        TB_Stu_Education, //教育
        [Description("15")]
        TB_Stu_Internship, //实习经历
        [Description("5")]
        TB_Stu_Language, //语言能力
        [Description("5")]
        TB_Stu_ITSkill, //IT技能
        [Description("5")]
        TB_Stu_Certificate, //证书
    }

    /// <summary>
    /// 企业认证状态
    /// </summary>
    public enum EtpCertificationStatus 
    {
        [Description("用户未处理")]
        NoHandle = 1,
        [Description("等待审核中")]
        Waiting,
        [Description("已认证")]
        Success,
        [Description("认证失败")]
        Fail
    }

    public enum WeChatSceneEventType
    {
        [Description("关注")]
        Subscribe = 1,
        [Description("取消关注")]
        UnSubscribe,
    }

    public enum MajorPositionMarkType 
    {
        [Description("置顶")]
        Stick = 1,
        [Description("下沉")]
        Sink,
        [Description("优推")]
        Recommand,
        [Description("取消优推")]
        UnRecommand,
        [Description("取消下沉")]
        UnSink,
        [Description("排序优先")]
        SortPrior,
        [Description("取消置顶")]
        UnStick,
        [Description("职位查询审核")]
        Check,
    }

    public enum QuickDateType
    { 
        [Description("全部时间")]
        All = 0,
        [Description("今天")]
        Today ,
        [Description("明天")]
        Tomorrow ,
        [Description("最近一周")]
        Week ,
        [Description("一周以后")]
        AfterWeek ,
    }

    #region 视频相关
    /// <summary>
    /// 企业上传视频 本地视频类型（用途），
    /// </summary>
    public enum EtpCloudVideoType
    {
        [Description("宣讲会视频")]
         CampusTalk = 1,
        [Description("常规视频")]
        Ordinary
    }


    /// <summary>
    /// 学生端上传视频 本地视频类型（用途），
    /// </summary>
    public enum  StuCloudVideoType
    {
        [Description("视频简历")]
        VideoResume = 1
    }
    /// <summary>
    /// 企业上传视频 第三方视频类型
    /// </summary>
    public enum OnlineVideoType
    {
        [Description("优酷")]
        YouKu = 1
    }
    /// <summary>
    /// 视频类型，上传途径
    /// </summary>
    public enum UploadVideoType
    {
        [Description("第三方上传")]
        Online = 1,
        [Description("本地上传")]
        Cloud
    }

    public enum VideoUploadStatus
    {
        [Description("上传好，但是还不能播放")]
        Processing = 0,
        [Description("上传中")]
        Uploading,
        [Description("上传好，并且已经可以播放")]
        CanPlay,
        [Description("找不到文件")]
        NoFind,
        [Description("待删除")]
        WaitToDelete,
        [Description("已删除")]
        Delete
    }
    #endregion

    /// <summary>
    /// 学校类别
    /// </summary>
    public enum SchoolType
    {
        [Description("综合")]
        A = 1,
        [Description("理工")]
        B,
        [Description("财经")]
        C,
        [Description("师范")]
        D,
        [Description("语言")]
        E,
        [Description("政法")]
        F,
        [Description("民族")]
        G,
        [Description("农林")]
        H,
        [Description("医药")]
        I,
        [Description("艺术")]
        J,
        [Description("体育")]
        K,
        [Description("军事")]
        L
    }



}
