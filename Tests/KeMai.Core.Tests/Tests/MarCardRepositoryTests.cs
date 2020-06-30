using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeMai.Tests.Domain.Entities;
using KeMai.Text;
using KeMai.Tests.Entities.Repositories;
using KeMai.Domain.Entities;
using System.Linq.Expressions;

namespace KeMai.Tests
{
    /// <summary>
    /// MarCardRepositoryTests 的摘要说明
    /// </summary>
    [TestClass]
    public class MarCardRepositoryTests
    {
        MarCardRepository repository = null;
        public MarCardRepositoryTests()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
            repository = new MarCardRepository();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion



        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Single_Test()
        {
            MarCard entity = CreateMarCard();
            repository.Insert(entity);
            var result = repository.Single(repository.Conn, entity.RowId);
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Insert_Test()
        {
            int insertResult = repository.Insert(CreateMarCard());
            Assert.IsTrue(insertResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void GetPaggingList_Test()
        {
            repository.Insert(CreateMarCard());
            int totalRows = 0;
            int custId = 1435700134;
            IEnumerable<MarCard> paggingList = repository.GetPaggingList(custId, null, null, null, null, ListResultsOrder.Ascending, null, null, out totalRows);
            Console.WriteLine("totalRows:" + totalRows);

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Update_Test()
        {
            int rowId = repository.Insert(CreateMarCard());
            MarCard entityToUpdate = new MarCard();
            entityToUpdate.UpdateTime = DateTime.Now;
            entityToUpdate.UpdateUser = 1;
            int updateResult = repository.Update(repository.Conn, entityToUpdate, rowId, new string[] { "UpdateTime", "UpdateUser" });
            Assert.IsTrue(updateResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void UpdateNonDefaults_Test()
        {
            int rowId = repository.Insert(CreateMarCard());
            MarCard entityToUpdate = new MarCard();
            entityToUpdate.UpdateTime = DateTime.Now;
            entityToUpdate.UpdateUser = 1;
            int updateResult = repository.UpdateNonDefaults(repository.Conn, entityToUpdate, rowId);
            Assert.IsTrue(updateResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void UpdateOnly_Test()
        {
            int rowId = repository.Insert(CreateMarCard());
            object objUpdate = new { UpdateTime = DateTime.Now, UpdateUser = 1 };
            int updateResult = repository.UpdateOnly(repository.Conn, objUpdate, rowId);
            Assert.IsTrue(updateResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void UpdateOnly_Expression_Test()
        {
            int rowId = repository.Insert(CreateMarCard());
            Expression<Func<MarCard>> updateFields = () => new MarCard()
            {
                UpdateTime = DateTime.Now,
                UpdateUser=1
            };
            int updateResult = repository.UpdateOnly(repository.Conn, updateFields, rowId);
            Assert.IsTrue(updateResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Delete_Test()
        {
            int rowId = repository.Insert(CreateMarCard());
            int deleteResult = repository.Delete(repository.Conn, rowId);
            Assert.IsTrue(deleteResult > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private MarCard CreateMarCard()
        {
            MarCard entity = new MarCard()
            {
                CustId = RandomData.GetInt(),//商户Id
                CardId = Guid.NewGuid().ToString("N"),//卡券ID代表一类卡券
                LogoUrl = RandomData.GetString(maxLength: 200),//卡券的商户logo，建议像素为300*300。
                CardType = RandomData.GetString(maxLength: 20),//卡券类型(CASH:代金券,DISCOUNT:折扣券,GIFT:兑换券,GROUPON:团购券)
                CodeType = RandomData.GetString(maxLength: 20),//核销码类型(CODE_TYPE_TEXT文 本 ； "CODE_TYPE_BARCODE"一维码 "CODE_TYPE_QRCODE"二维码 "CODE_TYPE_ONLY_QRCODE",二维码无code显示； "CODE_TYPE_ONLY_BARCODE",一维码无code显示；CODE_TYPE_NONE， 不显示code和条形码类型)
                Title = RandomData.GetString(maxLength: 30),//卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。
                SubTitle = RandomData.GetString(maxLength: 30),//显示在入口下方的提示语 ，仅在卡券状态正常(可以核销)时显示。
                Color = RandomData.GetString(maxLength: 16),//券颜色。按色彩规范标注填写Color010-Color100。
                Notice = RandomData.GetString(maxLength: 50),//卡券使用提醒，字数上限为16个汉字。
                Description = RandomData.GetString(maxLength: 1024),//卡券使用说明，字数上限为1024个汉字。
                Quantity = RandomData.GetInt(),//卡券库存的数量，上限为100000000。
                DateType = RandomData.GetString(maxLength: 20),//有效期类型(DATE_TYPE_FIX _TIME_RANGE 表示固定日期区间，DATE_TYPE_FIX_TERM表示 X天后生效,X天内有效)
                DateBeginTime = RandomData.GetDateTime(),//DateType为DATE_TYPE_FIX_TIME_RANGE时专用，表示起用时间
                DateEndTime = RandomData.GetDateTime(),//DateType为DATE_TYPE_FIX_TIME_RANGE时专用，表示结束时间 ， 建议设置为截止日期的23:59:59过期 
                DateFixedTerm = RandomData.GetInt(),//DateType为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天内有效，不支持填写0。
                DateFixedBeginTerm = RandomData.GetInt(),//DateType为DATE_TYPE_FIX_TERM时专用，表示自领取后多少天开始生效，领取后当天生效填写0。（单位为天）
                BindOpenid = RandomData.GetBool(),//是否指定用户领取，填写true或false 。默认为false。通常指定特殊用户群体 投放卡券或防止刷券时选择指定用户领取。
                ServicePhone = RandomData.GetString(maxLength: 20),//客服电话。
                GetLimit = RandomData.GetInt(),//每人可领券的数量限制,不填写默认为50。
                UseLimit = RandomData.GetInt(),//每人可核销的数量限制,不填写默认为50。
                CanShare = RandomData.GetBool(),//卡券领取页面是否可分享。
                CanGiveFriend = RandomData.GetBool(),//卡券是否可转赠
                AcceptCategory = RandomData.GetString(),//指定可用的商品类目
                RejectCategory = RandomData.GetString(),//指定不可用的商品类目，仅用于代金券类型 ，填入后将在券面拼写不适用于xxxx
                AcceptProduct = RandomData.GetString(),//指定可用的商品
                RejecProduct = RandomData.GetString(),//指定不可用的商品
                ReduceCost = RandomData.GetDecimal(0, (int)Math.Pow(2, 6)),//代金券专用，表示减免金额
                LeastCost = RandomData.GetDecimal(0, (int)Math.Pow(2, 6)),//满减门槛字段，可用于兑换券和代金券 ，填入后将在全面拼写消费满xx元可用。
                Discount = RandomData.GetDecimal(0, (int)Math.Pow(2, 6)),//折扣券专用，表示打折额度（百分比）
                CanUseWithOtheDiscount = RandomData.GetBool(),//不可以与其他类型共享门槛 ，填写false时系统将在使用须知里 拼写“不可与其他优惠共享”， 填写true时系统将在使用须知里 拼写“可与其他优惠共享”， 默认为true
                BusinessService = RandomData.GetString(maxLength: 200),//Arry类型 商家服务类型： BIZ_SERVICE_DELIVER 外卖服务； BIZ_SERVICE_FREE_PARK 停车位； BIZ_SERVICE_WITH_PET 可带宠物； BIZ_SERVICE_FREE_WIFI 免费wifi， 可多选
                /*
                {
                    type    否   string（24 ）	限制类型枚举值：支持填入 MONDAY 周一 TUESDAY 周二 WEDNESDAY 周三 THURSDAY 周四 FRIDAY 周五 SATURDAY 周六 SUNDAY 周日 此处只控制显示， 不控制实际使用逻辑，不填默认不显示
                    begin_hour   否   int 当前type类型下的起始时间（小时） ，如当前结构体内填写了MONDAY， 此处填写了10，则此处表示周一 10:00可用
                    begin_minute 否   int 当前type类型下的起始时间（分钟） ，如当前结构体内填写了MONDAY， begin_hour填写10，此处填写了59， 则此处表示周一 10:59可用
                    end_hour 否   int 当前type类型下的结束时间（小时） ，如当前结构体内填写了MONDAY， 此处填写了20， 则此处表示周一 10:00 - 20:00可用
                        end_minute   否   int 当前type类型下的结束时间（分钟） ，如当前结构体内填写了MONDAY， begin_hour填写10，此处填写了59， 则此处表示周一 10:59 - 00:59可用
                }
                */
                TimeLimit = @"[{""type"": ""MONDAY"", ""begin_hour"": 0,""end_hour"": 10,""begin_minute"": 10,""end_minute"": 59 }]",//JSON结构	使用时段限制，包含以下字段
                AbstractIntro = RandomData.GetString(maxLength: 24),//封面摘要简介
                AbstractIconUrlList = RandomData.GetString(maxLength: 200),//封面图片列表，仅支持填入一 个封面图片链接， 上传图片接口 上传获取图片获得链接，填写 非CDN链接会报错，并在此填入。 建议图片尺寸像素850*350
                ConsumeType = default(byte?),//核销方式(1:自助买单,2:自助核销,3:用扫码核销--二维码_条形码_仅卡券号)
                ConsumeNeedVerifyCode = RandomData.GetBool(),//自助核销是否启用验证码(消费者持券到店，须输入验证码才能核销卡券)
                ConsumeVerifyCode = RandomData.GetString(maxLength: 3),//自助核销验证码(消费者持券到店，须输入验证码才能核销卡券)
                ConsumeNeedRemarkAmount = RandomData.GetBool(),//自助核销是否启用备注交易金额(商户选择备注交易金额后，用户持券到店，须备注本次交易的金额才能成功销券，用于对账。)
                BranchNo = RandomData.GetString(),//适应的门店编号如:1234,5678
                Status = default(byte),//卡券状态(0:待审核,1:审核中,2:审核通过(待投放),3:审核未通过,4:已投放,5:下架)
                SendNum = RandomData.GetInt(),//投放数量
                UseNum = RandomData.GetInt(),//核销数量
                QuantityThreshold = RandomData.GetInt(),//库存阈值(当库存少于X提醒)
                QuantityThresholdNotificCount = RandomData.GetInt(),//库存阈值提醒次数
                QuantityThresholdNotificStatus = default(byte),//库存阈值提醒状态(0:禁用,1:启用)
                CreateTime = DateTime.Now,//创建时间
                CreateUser = RandomData.GetInt(),//创建用户
                UpdateTime = RandomData.GetDateTime(),//修改时间
                UpdateUser = RandomData.GetInt(),//修改用户
                DeleteFlag = false,//删除标志 1删除
                DeleteUser = RandomData.GetInt(),//删除用户
                DeleteTime = RandomData.GetDateTime(),//删除时间
            };
            return entity;
        }
    }
}
