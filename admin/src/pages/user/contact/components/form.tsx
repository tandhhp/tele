import { apiContactCreate, apiContactUpdate } from "@/services/contact";
import { apiDistrictOptions } from "@/services/settings/district";
import { apiJobKindOptions } from "@/services/settings/job-kind";
import { apiProvinceOptions } from "@/services/settings/province";
import { apiTransportOptions } from "@/services/settings/transport";
import { apiUserOptions } from "@/services/user";
import { apiTeamOptions } from "@/services/users/team";
import { DrawerForm, DrawerFormProps, ProFormInstance, ProFormSelect, ProFormText, ProFormTextArea } from "@ant-design/pro-components"
import { Col, message, Row } from "antd";
import { useEffect, useRef, useState } from "react";

type Props = DrawerFormProps & {
    reload?: () => void;
    data?: any;
}

const ContactForm: React.FC<Props> = (props) => {

    const formRef = useRef<ProFormInstance>();
    const [districtOptions, setDistrictOptions] = useState<any[]>([]);
    const [selectedProvinceId, setSelectedProvinceId] = useState<number | undefined>(undefined);
    const [teamId, setTeamId] = useState<number | undefined>(undefined);
    const [userOptions, setUserOptions] = useState<any[]>([]);

    useEffect(() => {
        if (teamId) {
            apiUserOptions({ teamId }).then((response) => {
                setUserOptions(response);
            });
        }
    }, [teamId]);

    useEffect(() => {
        if (selectedProvinceId) {
            apiDistrictOptions({ provinceId: selectedProvinceId }).then((response) => {
                setDistrictOptions(response);
            });
        }
    }, [selectedProvinceId]);

    const onFinish = async (values: any) => {
        if (props.data) {
            values.id = props.data.id;
            await apiContactUpdate(values);
        } else {
            await apiContactCreate(values);
        }
        formRef.current?.resetFields();
        message.success("Thao tác thành công");
        props.reload?.();
        return true;
    }

    return (
        <DrawerForm {...props} title="Liên hệ" formRef={formRef} onFinish={onFinish}>
            <Row gutter={[16, 16]}>
                <Col xs={24} md={12}>
                    <ProFormText name="name" label="Họ và tên" rules={[{ required: true, message: "Vui lòng nhập họ và tên" }]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name="gender" label="Giới tính" options={[
                        { label: "Nam", value: false },
                        { label: "Nữ", value: true }
                    ]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormText name="email" label="Email" rules={[{ type: "email", message: "Vui lòng nhập email hợp lệ" }]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormText name="phoneNumber" label="Số điện thoại" rules={[
                        {
                            pattern: /(03|05|07|08|09|01[2|6|8|9])+([0-9]{8})\b/,
                            message: "Vui lòng nhập số điện thoại hợp lệ"
                        },
                        {
                            required: true
                        }
                    ]} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`teamId`} label="Team" request={apiTeamOptions} showSearch
                        onChange={(value: number) => setTeamId(value)}
                    />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`userId`} label="Nhân viên phụ trách" showSearch options={userOptions} disabled={!teamId} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`provinceId`} label="Tỉnh/Thành phố" request={apiProvinceOptions}
                        onChange={(value: number) => setSelectedProvinceId(value)}
                        showSearch />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`districtId`} label="Xã/Phường" options={districtOptions} showSearch disabled={!selectedProvinceId} />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`jobKindId`} label="Nghề nghiệp" request={apiJobKindOptions} showSearch />
                </Col>
                <Col xs={24} md={12}>
                    <ProFormSelect name={`transportId`} label="Phương tiện" request={apiTransportOptions} showSearch />
                </Col>
            </Row>
            <ProFormTextArea name="note" label="Ghi chú" />
        </DrawerForm>
    )
}

export default ContactForm;