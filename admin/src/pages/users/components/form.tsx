import { DrawerForm, DrawerFormProps, ProFormInstance, ProFormText } from "@ant-design/pro-components"
import { useRef } from "react";

type Props = DrawerFormProps & {
    reload?: () => void;
    data?: any;
}

const UserForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>();

    return (
        <DrawerForm title="Tài khoản" {...props} formRef={formRef}>
            <ProFormText name="username" label="Tên đăng nhập"
                rules={[{ required: true, message: "Tên đăng nhập là bắt buộc" }]}
            />
            <div className="md:flex gap-4">
                <div className="flex-1">
                    <ProFormText name="email" label="Email" rules={[
                        {
                            type: "email",
                            message: "Email không hợp lệ"
                        }
                    ]} />
                </div>
                <div className="flex-1">
                    <ProFormText name={`phoneNumber`} label="Số điện thoại" rules={[
                        {
                            required: true,
                            message: "Số điện thoại là bắt buộc"
                        },
                        {
                            pattern: /^\d{10}$/,
                            message: "Số điện thoại không hợp lệ"
                        }
                    ]} />
                </div>
            </div>
            <ProFormText.Password name="password" label="Mật khẩu" rules={[{ required: true, message: "Mật khẩu là bắt buộc" }]} />
        </DrawerForm>
    )
}

export default UserForm;