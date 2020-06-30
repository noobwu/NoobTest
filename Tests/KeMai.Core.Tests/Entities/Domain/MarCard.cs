using System;
using KeMai.DataAnnotations;
using KeMai.Domain.Entities;

namespace KeMai.Tests.Domain.Entities
{

    /// <summary>
    /// 卡券
    /// </summary>
    [Serializable]
    [Alias("Mar_Card")]
    public class MarCard : Entity<int>, ISoftDelete
    {
        public MarCard()
        {

        }

        /// <summary>
        /// 自增长Id
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        [Alias("RowId")]
        [Required]
        public virtual int RowId { get; set; }



        ///<summary>
        /// 商户Id
        /// </summary>
        [Index]
        [Required]
        public virtual int CustId { get; set; }

        ///<summary>
        /// 卡券ID代表一类卡券
        /// </summary>
        [StringLength(32)]
        [Required]
        [IndexAttribute(Unique = true)]
        public virtual string CardId { get; set; }

        ///<summary>
        /// 卡券的商户logo，建议像素为300*300。
        /// </summary>
        [StringLength(200)]
        [Required]
        public virtual string LogoUrl { get; set; }

        ///<summary>
        /// 卡券类型(CASH:代金券,DISCOUNT:折扣券,GIFT:兑换券,GROUPON:团购券)
        /// </summary>
        [Index]
        [StringLength(20)]
        [Required]
        public virtual string CardType { get; set; }

        ///<summary>
        /// 核销码类型(CODE_TYPE_TEXT文 本 ； "CODE_TYPE_BARCODE"一维码 "CODE_TYPE_QRCODE"二维码 "CODE_TYPE_ONLY_QRCODE",二维码无code显示； "CODE_TYPE_ONLY_BARCODE",一维码无code显示；CODE_TYPE_NONE， 不显示code和条形码类型)
        /// </summary>
        [Index]
        [StringLength(20)]
        [Required]
        public virtual string CodeType { get; set; }

        ///<summary>
        /// 卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。
        /// </summary>
        [Index]
        [StringLength(30)]
        [Required]
        public virtual string Title { get; set; }

        ///<summary>
        /// 显示在入口下方的提示语 ，仅在卡券状态正常(可以核销)时显示。
        /// </summary>
        [StringLength(30)]
        public virtual string SubTitle { get; set; }

        ///<summary>
        /// 券颜色。按色彩规范标注填写Color010-Color100。
        /// </summary>
        [StringLength(16)]
        [Required]
        public virtual string Color { get; set; }

        ///<summary>
        /// 卡券使用提醒，字数上限为16个汉字。
        /// </summary>
        [StringLength(50)]
        [Required]
        public virtual string Notice { get; set; }

        ///<summary>
        /// 卡券使用说明，字数上限为1024个汉字。
        /// </summary>
        [StringLength(1024)]
        [Required]
        public virtual string Description { get; set; }

        ///<summary>
        /// 卡券库存的数量，上限为100000000。
        /// </summary>
        [Index]
        [Required]
        public virtual int Quantity { get; set; }

        ///<summary>
        /// 有效期类型(DATE_TYPE_FIX _TIME_RANGE 表示固定日期区间，DATE_TYPE_FIX_TERM表示 X天后生效,X天内有效)
        /// </summary>
        [Index]
        [StringLength(20)]
        [Required]
        public virtual string DateType { get; set; }

        ///<summary>
        /// DateType为DATE_TYPE_FIX_TIME_RANGE时专用，表示起用时间
        /// </summary>
        [Index]
        public virtual DateTime? DateBeginTime { get; set; }

        ///<summary>
        /// DateType为DATE_TYPE_FIX_TIME_RANGE时专用，表示结束时间 ， 建议设置为截止日期的23:59:59过期 
        /// </summary>
        [Index]
        public virtual DateTime? DateEndTime { get; set; }

        ///<summary>
        /// DateType为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天内有效，不支持填写0。
        /// </summary>
        public virtual int? DateFixedTerm { get; set; }

        ///<summary>
        /// DateType为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天开始生效，领取后当天生效填写0。（单位为天）
        /// </summary>
        public virtual int? DateFixedBeginTerm { get; set; }

        ///<summary>
        /// 是否指定用户领取，填写true或false 。默认为false。通常指定特殊用户群体 投放卡券或防止刷券时选择指定用户领取。
        /// </summary>
        public virtual bool? BindOpenid { get; set; }

        ///<summary>
        /// 客服电话。
        /// </summary>
        [StringLength(20)]
        public virtual string ServicePhone { get; set; }

        ///<summary>
        /// 每人可领券的数量限制,不填写默认为50。
        /// </summary>
        public virtual int? GetLimit { get; set; }

        ///<summary>
        /// 每人可核销的数量限制,不填写默认为50。
        /// </summary>
        public virtual int? UseLimit { get; set; }

        ///<summary>
        /// 卡券领取页面是否可分享。
        /// </summary>
        public virtual bool? CanShare { get; set; }

        ///<summary>
        /// 卡券是否可转赠
        /// </summary>
        public virtual bool? CanGiveFriend { get; set; }

        ///<summary>
        /// 指定可用的商品类目
        /// </summary>
        public virtual string AcceptCategory { get; set; }

        ///<summary>
        /// 指定不可用的商品类目，仅用于代金券类型 ，填入后将在券面拼写不适用于xxxx
        /// </summary>
        public virtual string RejectCategory { get; set; }

        ///<summary>
        /// 指定可用的商品
        /// </summary>
        public virtual string AcceptProduct { get; set; }

        ///<summary>
        /// 指定不可用的商品
        /// </summary>
        public virtual string RejecProduct { get; set; }

        ///<summary>
        /// 代金券专用，表示减免金额
        /// </summary>
        [Required]
        public virtual decimal ReduceCost { get; set; }

        ///<summary>
        /// 满减门槛字段，可用于兑换券和代金券 ，填入后将在全面拼写消费满xx元可用。
        /// </summary>
        [Required]
        public virtual decimal LeastCost { get; set; }

        ///<summary>
        /// 折扣券专用，表示打折额度（百分比）
        /// </summary>
        [Required]
        public virtual decimal Discount { get; set; }

        ///<summary>
        /// 不可以与其他类型共享门槛 ，填写false时系统将在使用须知里 拼写“不可与其他优惠共享”， 填写true时系统将在使用须知里 拼写“可与其他优惠共享”， 默认为true
        /// </summary>
        public virtual bool? CanUseWithOtheDiscount { get; set; }

        ///<summary>
        /// Arry类型 商家服务类型： BIZ_SERVICE_DELIVER 外卖服务； BIZ_SERVICE_FREE_PARK 停车位； BIZ_SERVICE_WITH_PET 可带宠物； BIZ_SERVICE_FREE_WIFI 免费wifi， 可多选
        /// </summary>
        [StringLength(200)]
        public virtual string BusinessService { get; set; }

        ///<summary>
        /// JSON结构	使用时段限制，包含以下字段
        ///  {
        ///     type 否   string（24 ）	限制类型枚举值：支持填入 MONDAY 周一 TUESDAY 周二 WEDNESDAY 周三 THURSDAY 周四 FRIDAY 周五 SATURDAY 周六 SUNDAY 周日 此处只控制显示， 不控制实际使用逻辑，不填默认不显示
        ///     begin_hour   否 int 当前type类型下的起始时间（小时） ，如当前结构体内填写了MONDAY， 此处填写了10，则此处表示周一 10:00可用
        ///     begin_minute 否 int 当前type类型下的起始时间（分钟） ，如当前结构体内填写了MONDAY， begin_hour填写10，此处填写了59， 则此处表示周一 10:59可用
        ///     end_hour 否 int 当前type类型下的结束时间（小时） ，如当前结构体内填写了MONDAY， 此处填写了20， 则此处表示周一 10:00-20:00可用
        ///     end_minute   否 int 当前type类型下的结束时间（分钟） ，如当前结构体内填写了MONDAY， begin_hour填写10，此处填写了59， 则此处表示周一 10:59-00:59可用
        ///   }
        /// </summary>
        [StringLength(1000)]
        public virtual string TimeLimit { get; set; }

        ///<summary>
        /// 封面摘要简介
        /// </summary>
        [StringLength(24)]
        [Required]
        public virtual string AbstractIntro { get; set; }

        ///<summary>
        /// 封面图片列表，仅支持填入一 个封面图片链接， 上传图片接口 上传获取图片获得链接，填写 非CDN链接会报错，并在此填入。 建议图片尺寸像素850*350
        /// </summary>
        [StringLength(200)]
        [Required]
        public virtual string AbstractIconUrlList { get; set; }

        ///<summary>
        /// 核销方式(1:自助买单,2:自助核销,3:用扫码核销--二维码_条形码_仅卡券号)
        /// </summary>
        public virtual byte? ConsumeType { get; set; }

        ///<summary>
        /// 自助核销是否启用验证码(消费者持券到店，须输入验证码才能核销卡券)
        /// </summary>
        public virtual bool? ConsumeNeedVerifyCode { get; set; }

        ///<summary>
        /// 自助核销验证码(消费者持券到店，须输入验证码才能核销卡券)
        /// </summary>
        [StringLength(3)]
        public virtual string ConsumeVerifyCode { get; set; }

        ///<summary>
        /// 自助核销是否启用备注交易金额(商户选择备注交易金额后，用户持券到店，须备注本次交易的金额才能成功销券，用于对账。)
        /// </summary>
        public virtual bool? ConsumeNeedRemarkAmount { get; set; }

        ///<summary>
        /// 适应的门店编号如:1234,5678
        /// </summary>
        public virtual string BranchNo { get; set; }

        ///<summary>
        /// 卡券状态(0:待审核,1:审核中,2:审核通过(待投放),3:审核未通过,4:已投放,5:下架)
        /// </summary>
        [Index]
        [Required]
        public virtual byte Status { get; set; }

        ///<summary>
        /// 投放数量
        /// </summary>
        [Required]
        public virtual int SendNum { get; set; }

        ///<summary>
        /// 核销数量
        /// </summary>
        [Required]
        public virtual int UseNum { get; set; }

        ///<summary>
        /// 库存阈值(当库存少于X提醒)
        /// </summary>
        [Required]
        public virtual int QuantityThreshold { get; set; }

        ///<summary>
        /// 库存阈值提醒次数
        /// </summary>
        [Required]
        public virtual int QuantityThresholdNotificCount { get; set; }

        ///<summary>
        /// 库存阈值提醒状态(0:禁用,1:启用)
        /// </summary>
        [Required]
        public virtual byte QuantityThresholdNotificStatus { get; set; }

        ///<summary>
        /// 创建时间
        /// </summary>
        [Required]
        public virtual DateTime CreateTime { get; set; }

        ///<summary>
        /// 创建用户
        /// </summary>
        [Required]
        public virtual int CreateUser { get; set; }

        ///<summary>
        /// 修改时间
        /// </summary>
        [Required]
        public virtual DateTime UpdateTime { get; set; }

        ///<summary>
        /// 修改用户
        /// </summary>
        [Required]
        public virtual int UpdateUser { get; set; }

        ///<summary>
        /// 删除标志 1删除
        /// </summary>
        [Required]
        public virtual bool DeleteFlag { get; set; }

        ///<summary>
        /// 删除用户
        /// </summary>
        [Required]
        public virtual int DeleteUser { get; set; }

        ///<summary>
        /// 删除时间
        /// </summary>
        [Required]
        public virtual DateTime DeleteTime { get; set; }
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        [Ignore]
        public override int Id { get { return RowId; } set { RowId = value; } }
        /// <summary>
        /// 获取主键的属性名称
        /// </summary>
        /// <returns></returns>
        public override string GetPKPropertyName()
        {
            return "RowId";
        }

    }
}
