using System;
using AppCore.Runtime.UserData;
using NUnit.Framework;

namespace AppCore.Tests
{
    public class TestUserData
    {
        [Test, Description("ユーザーIDのテスト")]
        public void TestValidateUserId()
        {
            var sampleGuid = new Guid("8da0e069-05a6-4433-a52d-0dac934cfdb1");
            
            // 正常チェック
            var userId = UserId.Create(sampleGuid.ToString("D"));
            Assert.That(userId.Value, Is.EqualTo(sampleGuid.ToString("D")));
            
            // 異常チェック　null or 空文字
            var ex = Assert.Throws<ArgumentException>(() => UserId.Create(""));
            Assert.That(ex.Message, Is.EqualTo("値がnull または 空です。"));
            
            // 異常チェック　フォーマット不正
            var testUserId = sampleGuid.ToString("N");
            ex = Assert.Throws<ArgumentException>(() => UserId.Create(testUserId));
            Assert.That(ex.Message, Is.EqualTo($"{testUserId}のフォーマットが不正です。"));

            testUserId = sampleGuid.ToString("D");
            testUserId += "1ush";
            ex = Assert.Throws<ArgumentException>(() => UserId.Create(testUserId));
            Assert.That(ex.Message, Is.EqualTo($"{testUserId}のフォーマットが不正です。"));
        }

        [Test, Description("ユーザー名のテスト")]
        public void TestValidateUserName()
        {
            var sampleName = "test";
            
            // 正常チェック
            var userName = UserName.Create(sampleName);
            Assert.That(userName.Value, Is.EqualTo(sampleName));
            
            // 異常チェック　null or 空文字
            var ex = Assert.Throws<ArgumentException>(() => UserName.Create(""));
            Assert.That(ex.Message, Is.EqualTo("値がnull または 空です。"));
            
            // 異常チェック　文字数不正
            var testUserName = "12345678901";
            ex = Assert.Throws<ArgumentException>(() => UserName.Create(testUserName));
            Assert.That(ex.Message, Is.EqualTo($"名前の長さは 1 文字以上 10 文字以内でなければなりません。"));
        }
    }
}
