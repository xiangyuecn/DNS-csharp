using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DNS {
	/// <summary>
	/// 封装的一些通用方法
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// 用join字符串拼接数组内元素,list[0]+join+list[1]...
		/// getFn:返回格式化后的字符串，比如"aa"，返回null不拼接此字符串
		/// </summary>
		static public string join(this IEnumerable list, string join, Func<object, string> getFn = null) {
			StringBuilder str = new StringBuilder();
			bool start = false;
			foreach (object o in list) {
				if (getFn != null) {
					string val = getFn(o);
					if (val != null) {
						if (start) {
							str.Append(join);
						}
						str.Append(val);
					} else {
						continue;
					}
				} else {
					if (start) {
						str.Append(join);
					}
					str.Append(o);
				}
				start = true;
			}
			return str.ToString();
		}
	}

	public class Result : IResult<object> {

	}
	public class Result<T> : IResult<T> {

	}
	public abstract class IResult<T> {
		public Dictionary<string, object> Json { get { return json; } }
		protected Dictionary<string, object> json;
		public IResult() {
			json = new Dictionary<string, object>();

			ErrorMessage = "";
			ServerErrorMessage = "";
		}

		protected bool isErr = false;
		protected bool isSevErr = false;
		public string ErrorMessage { get; set; }
		public string ServerErrorMessage { get; set; }
		public T Value {
			get {
				object val;
				json.TryGetValue("v", out val);
				return val == null ? default(T) : (T)val;
			}
			set {
				json["v"] = value;
			}
		}

		public IResult<T> buildResult() {
			json["c"] = isErr ? 1 : 0;
			json["m"] = ErrorMessage;
			if (isSevErr) {
				json["m_sev"] = ServerErrorMessage;
			}

			return this;
		}
		/// <summary>
		/// 运行过程中是否出现错误，如果出错后续业务不应该被执行
		/// </summary>
		public bool IsError {
			get { return isErr; }
		}
		/// <summary>
		/// 运行异常，比如无法处理的捕获异常
		/// </summary>
		/// <param name="message">用户提示</param>
		/// <param name="serverMessage">服务器错误详细信息</param>
		public void fail(string message, string serverMessage) {
			isSevErr = true;
			ServerErrorMessage = serverMessage;
			error(message);
		}
		/// <summary>
		/// 出现错误，给用户友好提示
		/// </summary>
		/// <param name="message">用户提示</param>
		public void error(string message) {
			isErr = true;
			ErrorMessage = message;
		}

		/// <summary>
		/// 把错误信息设置到另外一个对象，包括服务器错误，如果result已经有错将不会复制，新的错误会添加到现有错误前面
		/// </summary>
		public void errorTo<X>(IResult<X> result, string newErr = "", string newSrvErr = "") {
			if (result.isErr || !isErr) {
				return;
			}
			var err = String.IsNullOrEmpty(newErr) ? "" : newErr + "\nby\n";
			err += ErrorMessage;
			var srvErr = String.IsNullOrEmpty(newErr) ? "" : newSrvErr + "\nby\n";

			if (isSevErr || srvErr != "") {
				srvErr += ServerErrorMessage;

				result.fail(err, srvErr);
			} else {
				result.error(err);
			}
		}
	}
}
