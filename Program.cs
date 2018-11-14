using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNS {
	class Program {
		//测试用例
		static void DNSTest() {
			Console.WriteLine("【zhujiwu.com】");
			Console.WriteLine("A: " + new DNS_A().QueryAll("zhujiwu.com").Value.join(", "));
			Console.WriteLine("CNAME: "+new DNS_CNAME().QueryAll("zhujiwu.com").Value.join(", "));
			var cname = new DNS_CNAME().QueryOne("zhujiwu.com").Value;

			Console.WriteLine("A(From CNAME): " + new DNS_A().QueryAll(cname).Value.join(", "));

			Console.WriteLine();
			Console.WriteLine("【qq.com】");

			var a = new DNS_A().QueryAll("qq.com").Value;
			Console.WriteLine("A: " + a.join(", "));
			Console.WriteLine("NS: " + new DNS_NS().QueryAll("qq.com").Value.join(", "));
			Console.WriteLine("MX: " + new DNS_MX().QueryAll("qq.com").Value.join(", "));
			Console.WriteLine("TXT: " + new DNS_TXT().QueryAll("qq.com").Value.join(", "));

			Console.WriteLine("");

			//114.114.115.115 dns反查
			Console.WriteLine("PTR: " + new DNS_PTR().QueryAll("114.114.115.115").Value.join(", "));

			//扩展的查询方法
			Console.WriteLine("DNAME: " + new DNS_DNAME().QueryAll("qq.com").Value.join(", "));
		}


		//扩展查询类型
		public class DNS_DNAME : DNSBase {
			static DNS_DNAME() {
				DNSBase.RegisterType("DNAME", 0x0027);
			}

			[StructLayout(LayoutKind.Sequential)]
			private class DNAME : Record {
				public IntPtr pNameHost;
			}

			protected override Type RecordType { get { return typeof(DNAME); } }
			protected override string GetVal(object obj) {
				return Marshal.PtrToStringUni(((DNAME)obj).pNameHost);
			}
		}



		static void Main(string[] args) {
			Console.WriteLine("---------------------------------------------------------");
			Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆ DNS测试 ◆◆◆◆◆◆◆◆◆◆◆◆");
			Console.WriteLine("---------------------------------------------------------");

			DNSTest();

			Console.WriteLine("-------------------------------------------------------------");
			Console.WriteLine("◆◆◆◆◆◆◆◆◆◆◆◆ 回车退出... ◆◆◆◆◆◆◆◆◆◆◆◆");
			Console.WriteLine();
			Console.ReadLine();
		}
	}
}
